/*
AssetValidator 
Copyright (c) 2018 Jeff Campbell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using JCMG.AssetValidator.Editor.Config;
using JCMG.AssetValidator.Editor.Meta;
using JCMG.AssetValidator.Editor.Utility;
using JCMG.AssetValidator.Editor.Validators;
using JCMG.AssetValidator.Editor.Validators.Output;
using JCMG.AssetValidator.UnitTestObjects;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Window
{
    public sealed class AssetValidatorRunner : IDisposable
    {
        private enum RunningState
        {
            InstanceState,
            CrossSceneState,
            ProjectAssetState
        }

        private RunningState _runningState;
        private int _sceneProgress;
        private string _currentScenePath;
        private bool _isRunning;
        private double _runningTime;

        private readonly IList<string> _scenePaths;
        private readonly AssetValidatorLogger _logger;
        private readonly ClassTypeCache _cache;

        private ProjectAssetValidatorManager _projectAssetValidatorManager;
        private CrossSceneValidatorManager _crossSceneValidatorManager;
        private ActiveSceneValidatorManager _activeSceneValidatorManager;

        public AssetValidatorRunner(AssetValidatorLogger logger, SceneValidationMode vmode)
        {
            _scenePaths = AssetValidatorUtility.GetScenePaths(vmode);

            _logger = logger;

            _cache = new ClassTypeCache();

            // Ensure any unit test types do not get picked up for validation.
            _cache.IgnoreType<Monobehavior2>();
            _cache.IgnoreAttribute<OnlyIncludeInTestsAttribute>();

            // Find all objects for validation
            _cache.AddTypeWithAttribute<MonoBehaviour, ValidateAttribute>();

            // Add all disabled logs for this run
            AssetValidatorOverrideConfig.FindOrCreate().AddDisabledLogs(logger);

            _isRunning = true;
            _runningTime = EditorApplication.timeSinceStartup;
        }

        public void EnableCrossSceneValidation()
        {
            _crossSceneValidatorManager = new CrossSceneValidatorManager(_logger);
        }

        public void EnableProjectAssetValidation()
        {
            _projectAssetValidatorManager = new ProjectAssetValidatorManager(_cache, _logger);
        }

        public bool IsComplete()
        {
            return (_scenePaths == null || _sceneProgress >= _scenePaths.Count) &&
                    (_crossSceneValidatorManager == null || _crossSceneValidatorManager.IsComplete()) &&
                    (_projectAssetValidatorManager == null || _projectAssetValidatorManager.IsComplete());
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        public float GetProgress()
        {
            switch (_runningState)
            {
                case RunningState.InstanceState:
                    return _activeSceneValidatorManager != null ? _activeSceneValidatorManager.GetProgress() : 1f;
                case RunningState.CrossSceneState:
                    return _crossSceneValidatorManager != null ? _crossSceneValidatorManager.GetProgress() : 1f;
                case RunningState.ProjectAssetState:
                    return _projectAssetValidatorManager != null ? _projectAssetValidatorManager.GetProgress() : 1f;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns a message describing the runtime of the AssetValidatorRunner
        /// </summary>
        /// <returns></returns>
        public string GetProgressMessage()
        {
            return string.Format("Running for {0:F} seconds...", EditorApplication.timeSinceStartup - _runningTime);
        }

        public void Run()
        {
            if (_projectAssetValidatorManager != null)
            {
                _projectAssetValidatorManager.Search();
                _projectAssetValidatorManager.ValidateAll();
            }

            while (HasScenesToSearch())
            {
                _currentScenePath = _scenePaths[_sceneProgress];

                // Only load the next scene if we are not already in it
                if (EditorSceneManager.GetActiveScene().path != _currentScenePath)
                    EditorSceneManager.OpenScene(_currentScenePath);

                using (var activeSceneManager = new ActiveSceneValidatorManager(_cache, _logger))
                {
                    activeSceneManager.Search();
                    activeSceneManager.ValidateAll();
                }

                if (_crossSceneValidatorManager != null)
                    _crossSceneValidatorManager.Search();

                ++_sceneProgress;
            }

            if(_crossSceneValidatorManager != null)
                _crossSceneValidatorManager.ValidateAll();
        }

        public void ContinueRunning()
        {
            ContinueProjectAssetValidation();
            ContinueSceneValidation();

            _isRunning = !IsComplete();
        }

        private void ContinueProjectAssetValidation()
        {
            if (_projectAssetValidatorManager == null || 
                (_projectAssetValidatorManager.IsSearchComplete() && 
                 _projectAssetValidatorManager.IsComplete()))
                return;

            if (!_projectAssetValidatorManager.IsSearchComplete())
            {
                _projectAssetValidatorManager.ContinueSearch();
                return;
            }

            if (_projectAssetValidatorManager.ContinueValidation())
            {
                if (AssetValidatorUtility.IsDebugging)
                    Debug.LogFormat("Progress: {0:P2}% for validating project assets",
                        _projectAssetValidatorManager.GetProgress());
            }
            else
            {
                if (AssetValidatorUtility.IsDebugging)
                    Debug.LogFormat("ProjectAssetValidatorManager has completed validating project assets",
                        _projectAssetValidatorManager.GetProgress());
            }
        }

        private void ContinueSceneValidation()
        {
            if (!HasScenesToSearch())
            {
                if (_crossSceneValidatorManager == null) return;

                _runningState = RunningState.CrossSceneState;

                if (_crossSceneValidatorManager.ContinueValidation())
                {
                    if (AssetValidatorUtility.IsDebugging)
                        Debug.LogFormat("Progress: {0:P2}% for validating cross scene information",
                            _crossSceneValidatorManager.GetProgress());
                }
                else
                {
                    if (AssetValidatorUtility.IsDebugging)
                        Debug.LogFormat("CrossSceneValidatorManager has completed validating information gathered across all targeted Scene(s).",
                            _crossSceneValidatorManager.GetProgress());
                }

                return;
            }

            if (_activeSceneValidatorManager == null)
            {
                _currentScenePath = _scenePaths[_sceneProgress];

                // Only load the next scene if we are not already in it
                if (EditorSceneManager.GetActiveScene().path != _currentScenePath)
                    EditorSceneManager.OpenScene(_currentScenePath);

                if(_activeSceneValidatorManager != null)
                    _activeSceneValidatorManager.Dispose();

                _activeSceneValidatorManager = new ActiveSceneValidatorManager(_cache, _logger);
                _activeSceneValidatorManager.Search();

                if(_crossSceneValidatorManager != null)
                    _crossSceneValidatorManager.Search();
            }

            _runningState = RunningState.InstanceState;

            if (_activeSceneValidatorManager.ContinueValidation())
            {
                if(AssetValidatorUtility.IsDebugging)
                    Debug.LogFormat("Progress: {0:P2}% for scene instance types {1}/{2}: [{3}]", 
                        _activeSceneValidatorManager.GetProgress(), _sceneProgress, _scenePaths.Count, _currentScenePath);
            }
            else
            {
                if (AssetValidatorUtility.IsDebugging)
                    Debug.LogFormat("ActiveSceneValidator has completed validating Scene {0}/{1}: [{2}]", 
                        _sceneProgress, _scenePaths.Count, _currentScenePath);

                _sceneProgress++;

                _activeSceneValidatorManager.Dispose();
                _activeSceneValidatorManager = null;
            }
        }

        /// <summary>
        /// Returns whether or not there are scenes to search AND whether we have searched them allready.
        /// </summary>
        /// <returns></returns>
        private bool HasScenesToSearch()
        {
            return _scenePaths != null && _sceneProgress < _scenePaths.Count;
        }

        /// <summary>
        /// TODO Infer or get this in a more intelligent, specific way
        /// </summary>
        /// <returns></returns>
        private string GetCurrentTarget()
        {
            return HasScenesToSearch() ? _currentScenePath : "Project";
        }

        #region IDisposable

        public void Dispose()
        {
            if(_activeSceneValidatorManager != null)
                _activeSceneValidatorManager.Dispose();

            if(_crossSceneValidatorManager != null)
                _crossSceneValidatorManager.Dispose();

            if(_projectAssetValidatorManager != null)
                _projectAssetValidatorManager.Dispose();
        }

        #endregion
    }
}
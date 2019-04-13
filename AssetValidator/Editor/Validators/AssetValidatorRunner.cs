/*
MIT License

Copyright (c) 2019 Jeff Campbell

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
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="AssetValidatorRunner"/> is a utility class meant to help aid in running multiple types of
	/// validators in sequence for a session.
	/// </summary>
	internal sealed class AssetValidatorRunner
	{
		/// <summary>
		/// The current mode for the validation session.
		/// </summary>
		private enum Mode
		{
			/// <summary>
			/// The current active scene is currently being validated.
			/// </summary>
			ActiveScene,

			/// <summary>
			/// The cross scene aggregated results are being validated.
			/// </summary>
			CrossScene,

			/// <summary>
			/// Objects in the Project Assets folder are being validated.
			/// </summary>
			ProjectAssets
		}

		private Mode _mode;
		private int _sceneProgress;
		private string _currentScenePath;
		private bool _isRunning;
		private readonly double _runningTime;

		private readonly IReadOnlyList<string> _scenePaths;
		private readonly LogCache _logCache;
		private readonly ClassTypeCache _cache;

		private ProjectAssetValidatorManager _projectAssetValidatorManager;
		private CrossSceneValidatorManager _crossSceneValidatorManager;
		private ActiveSceneValidatorManager _activeSceneValidatorManager;

		private const string RunningMessageFormat = "Running for {0:F} seconds...";
		private const string ProjectValidationProgressMessageFormat =
			"Progress: {0:P2}% for validating project assets";
		private const string ProjectValidationCompleted =
			"ProjectAssetValidatorManager has completed validating project assets";
		private const string CrossSceneValidationProgressFormat =
			"Progress: {0:P2}% for validating cross scene information";
		private const string CrossSceneValidationCompleted =
			"CrossSceneValidatorManager has completed validating information gathered across all targeted Scene(s).";
		private const string SceneValidationProgressFormat =
			"Progress: {0:P2}% for scene instance types {1}/{2}: [{3}]";
		private const string ScenesValidationProgressFormat =
			"ActiveSceneValidator has completed validating Scene {0}/{1}: [{2}]";

		/// <summary>
		/// Constructor that accepts the <see cref="LogCache"/> <paramref name="logCache"/> that logs will be
		/// stored in and the <see cref="SceneValidationMode"/> <paramref name="validationMode"/> that scenes
		/// will be validated in (if any).
		/// </summary>
		/// <param name="logCache"></param>
		/// <param name="validationMode"></param>
		public AssetValidatorRunner(LogCache logCache, SceneValidationMode validationMode)
		{
			_scenePaths = ProjectTools.GetScenePaths(validationMode);

			_logCache = logCache;

			_cache = new ClassTypeCache();

			// Ensure any unit test types do not get picked up for validation.
			_cache.IgnoreType<MonoBehaviourTwo>();
			_cache.IgnoreAttribute<OnlyIncludeInTestsAttribute>();

			// Find all objects for validation
			_cache.AddTypeWithAttribute<MonoBehaviour, ValidateAttribute>();

			// Add all disabled logs for this run
			AssetValidatorOverrideConfig.FindOrCreate().AddDisabledLogs(logCache);

			_isRunning = true;
			_runningTime = EditorApplication.timeSinceStartup;
		}

		/// <summary>
		/// Enables cross-scene validation.
		/// </summary>
		public void EnableCrossSceneValidation()
		{
			_crossSceneValidatorManager = new CrossSceneValidatorManager(_logCache);
		}

		/// <summary>
		/// Enables project asset validation.
		/// </summary>
		public void EnableProjectAssetValidation()
		{
			_projectAssetValidatorManager = new ProjectAssetValidatorManager(_cache, _logCache);
		}

		/// <summary>
		/// Returns true if the validation run has completed, otherwise false.
		/// </summary>
		/// <returns></returns>
		public bool IsComplete()
		{
			return (_scenePaths == null || _sceneProgress >= _scenePaths.Count) &&
			       (_crossSceneValidatorManager == null || _crossSceneValidatorManager.IsComplete()) &&
			       (_projectAssetValidatorManager == null || _projectAssetValidatorManager.IsComplete());
		}

		/// <summary>
		/// Is validation currently running or not?
		/// </summary>
		/// <returns></returns>
		public bool IsRunning()
		{
			return _isRunning;
		}

		/// <summary>
		/// Returns a scalar value [0-1] representing the progress of the current validation area.
		/// </summary>
		/// <returns></returns>
		public float GetProgress()
		{
			switch (_mode)
			{
				case Mode.ActiveScene:
					return _activeSceneValidatorManager != null
						? _activeSceneValidatorManager.GetProgress()
						: 1f;
				case Mode.CrossScene:
					return _crossSceneValidatorManager != null
						? _crossSceneValidatorManager.GetProgress()
						: 1f;
				case Mode.ProjectAssets:
					return _projectAssetValidatorManager != null
						? _projectAssetValidatorManager.GetProgress()
						: 1f;
				default:
					throw new ArgumentOutOfRangeException(Enum.GetName(typeof(Mode), _mode));
			}
		}

		/// <summary>
		/// Returns a message describing the progress of the current validation area.
		/// </summary>
		/// <returns></returns>
		public string GetProgressMessage()
		{
			return string.Format(RunningMessageFormat, EditorApplication.timeSinceStartup - _runningTime);
		}

		/// <summary>
		/// Synchronously runs validation on all of the configured areas.
		/// </summary>
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
				if (SceneManager.GetActiveScene().path != _currentScenePath)
				{
					EditorSceneManager.OpenScene(_currentScenePath);
				}

				var activeSceneManager = new ActiveSceneValidatorManager(_cache, _logCache);
				activeSceneManager.Search();
				activeSceneManager.ValidateAll();

				_crossSceneValidatorManager?.Search();

				++_sceneProgress;
			}

			_crossSceneValidatorManager?.ValidateAll();

			_isRunning = true;
		}

		/// <summary>
		/// Runs a synchronous step of the current validation area and marks the runner as complete if all
		/// areas have been completed.
		/// </summary>
		public void ContinueRunning()
		{
			ContinueProjectAssetValidation();
			ContinueSceneValidation();

			_isRunning = !IsComplete();
		}

		/// <summary>
		/// Runs a synchronous step of project asset validation.
		/// </summary>
		private void ContinueProjectAssetValidation()
		{
			if (_projectAssetValidatorManager == null ||
			    (_projectAssetValidatorManager.IsSearchComplete() &&
			     _projectAssetValidatorManager.IsComplete()))
			{
				return;
			}

			if (!_projectAssetValidatorManager.IsSearchComplete())
			{
				_projectAssetValidatorManager.ContinueSearch();
				return;
			}

			if (_projectAssetValidatorManager.ContinueValidation())
			{
				if (ProjectTools.IsDebugging)
				{
					Debug.LogFormat(
						ProjectValidationProgressMessageFormat,
						_projectAssetValidatorManager.GetProgress());
				}
			}
			else
			{
				if (ProjectTools.IsDebugging)
				{
					Debug.LogFormat(ProjectValidationCompleted);
				}
			}
		}

		/// <summary>
		/// Runs a synchronous step of scene validation (cross and/or active if enabled).
		/// </summary>
		private void ContinueSceneValidation()
		{
			if (!HasScenesToSearch())
			{
				if (_crossSceneValidatorManager == null)
				{
					return;
				}

				_mode = Mode.CrossScene;

				if (_crossSceneValidatorManager.ContinueValidation())
				{
					if (ProjectTools.IsDebugging)
					{
						Debug.LogFormat(
							CrossSceneValidationProgressFormat,
							_crossSceneValidatorManager.GetProgress());
					}
				}
				else
				{
					if (ProjectTools.IsDebugging)
					{
						Debug.LogFormat(CrossSceneValidationCompleted);
					}
				}

				return;
			}

			if (_activeSceneValidatorManager == null)
			{
				_currentScenePath = _scenePaths[_sceneProgress];

				// Only load the next scene if we are not already in it
				if (SceneManager.GetActiveScene().path != _currentScenePath)
				{
					EditorSceneManager.OpenScene(_currentScenePath);
				}

				_activeSceneValidatorManager = new ActiveSceneValidatorManager(_cache, _logCache);
				_activeSceneValidatorManager.Search();

				_crossSceneValidatorManager?.Search();
			}

			_mode = Mode.ActiveScene;

			if (_activeSceneValidatorManager.ContinueValidation())
			{
				if (ProjectTools.IsDebugging)
				{
					Debug.LogFormat(
						SceneValidationProgressFormat,
						_activeSceneValidatorManager.GetProgress(),
						_sceneProgress,
						_scenePaths.Count,
						_currentScenePath);
				}
			}
			else
			{
				if (ProjectTools.IsDebugging)
				{
					Debug.LogFormat(
						ScenesValidationProgressFormat,
						_sceneProgress,
						_scenePaths.Count,
						_currentScenePath);
				}

				_sceneProgress++;

				_activeSceneValidatorManager = null;
			}
		}

		/// <summary>
		/// Returns true whether or not there are scenes to search AND whether we have searched them already.
		/// </summary>
		/// <returns></returns>
		private bool HasScenesToSearch()
		{
			return _scenePaths != null && _sceneProgress < _scenePaths.Count;
		}
	}
}

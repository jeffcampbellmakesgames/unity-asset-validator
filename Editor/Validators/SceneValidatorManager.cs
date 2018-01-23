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
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Validators
{
    /// <summary>
    /// SceneValidatorManager handles the loading, validation, and unloading of scenes.
    /// </summary>
    public sealed class SceneValidatorManager : IDisposable
    {
        private readonly BaseValidatorManager _validatorManager;
        private readonly IList<string> _scenePaths;

        private int progress = 0;

        public SceneValidatorManager(BaseValidatorManager validatorManager, IList<string> scenePaths)
        {
            _validatorManager = validatorManager;
            _scenePaths = scenePaths;
        }

        public bool CanContinueValidating()
        {
            if (progress < _scenePaths.Count)
                return true;

            return false;
        }

        public void ContinueValidating()
        {
            var path = _scenePaths[progress++];

            UpdateProgress(path);

            if (path == string.Empty)
            {
                Debug.LogWarning("The current scene must be saved in the project before it can be validated.");
                return;
            }

            EditorSceneManager.OpenScene(path);

            _validatorManager.Search();
            _validatorManager.ValidateAll();
        }

        private void UpdateProgress(string path)
        {
            EditorUtility.DisplayProgressBar("AssetValidator",
                string.Format("Searching and Validating Scene: [{0}]", path),
                progress / (float)_scenePaths.Count);
        }

        public void Dispose()
        {
            EditorUtility.ClearProgressBar();
        }
    }
}
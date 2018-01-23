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
using JCMG.AssetValidator.Editor.Validators;
using JCMG.AssetValidator.Editor.Validators.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace JCMG.AssetValidator.Editor.Utility
{
    public static class AssetValidatorUtility
    {
        public static OutputFormat EditorOuputFormat = OutputFormat.None;
        public static string EditorFilename = "asset_validator_results";

        public static bool IsDebugging
        {
            get { return EditorPrefs.GetBool(ASSET_VALIDATOR_IS_DEBUGGING, false); }
            set
            {
                EditorPrefs.SetBool(ASSET_VALIDATOR_IS_DEBUGGING, value);
            }
        }
        public const string ASSET_VALIDATOR_IS_DEBUGGING = "ASSET_VALIDATOR_IS_DEBUGGING";

        public static IList<string> GetScenePaths(SceneValidationMode vmode)
        {
            switch (vmode)
            {
                case SceneValidationMode.None:
                    return new string[0];
                case SceneValidationMode.ActiveScene:
                    return new[] {EditorSceneManager.GetActiveScene().path};
                case SceneValidationMode.AllScenes:
                    return GetAllScenePathsInProject();
                case SceneValidationMode.AllBuildScenes:
                    return GetAllScenePathsInBuildSettings();
                case SceneValidationMode.AllBuildAndAssetBundleScenes:
                    var finalScenes = GetAllScenePathsInAssetBundles();
                    finalScenes.AddRange(GetAllScenePathsInBuildSettings());

                    return finalScenes;
                default:
                    throw new ArgumentOutOfRangeException("vmode", vmode, null);
            }
        }

        public static List<string> GetAllScenePathsInProject()
        {
            var assetPaths = AssetDatabase.FindAssets("t:scene");
            var scenePaths = new List<string>();

            for (var i = 0; i < assetPaths.Length; i++)
                scenePaths.Add(AssetDatabase.GUIDToAssetPath(assetPaths[i]));

            return scenePaths;
        }

        public static List<string> GetAllScenePathsInAssetBundles()
        {
            var sceneNames = new List<string>();
            var allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            for (var i = 0; i < allAssetBundleNames.Length; i++)
            {
                var assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(allAssetBundleNames[i]);
                for (var j = 0; j < assetNames.Length; j++)
                {
                    if (assetNames[j].Contains(".unity"))
                        sceneNames.Add(assetNames[j]);
                }
            }

            return sceneNames;
        }

        public static IList<string> GetAllScenePathsInBuildSettings()
        {
            return EditorBuildSettings.scenes.Select(x => x.path).ToList();
        }

        public static string GetValidatorDescription(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(ValidatorDescriptionAttribute), false) as ValidatorDescriptionAttribute[];
            return attrs != null && attrs.Length > 0
                ? attrs[0].Description
                : string.Empty;
        }
    }
}

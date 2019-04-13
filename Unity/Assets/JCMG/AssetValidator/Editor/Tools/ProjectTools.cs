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
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// Helper methods for dealing with Unity project assets and folder.
	/// </summary>
	public static class ProjectTools
	{
		/// <summary>
		/// Has the user set their preference for debugging to true or false?
		/// </summary>
		internal static bool IsDebugging
		{
			get { return EditorPrefs.GetBool(ASSET_VALIDATOR_IS_DEBUGGING, false); }
			set { EditorPrefs.SetBool(ASSET_VALIDATOR_IS_DEBUGGING, value); }
		}

		private const string ASSET_VALIDATOR_IS_DEBUGGING = "ASSET_VALIDATOR_IS_DEBUGGING";

		private const string SceneAssetExtension = ".unity";
		private const string ProjectSearchFilterForScenes = "t:scene";

		/// <summary>
		/// Returns a read-only list of scene paths based on the passed <see cref="SceneValidationMode"/>
		/// <paramref name="validationMode"/>.
		/// </summary>
		/// <param name="validationMode"></param>
		/// <returns></returns>
		public static IReadOnlyList<string> GetScenePaths(SceneValidationMode validationMode)
		{
			switch (validationMode)
			{
				case SceneValidationMode.None:
					return new string[0];

				case SceneValidationMode.ActiveScene:
					return new[] {SceneManager.GetActiveScene().path};

				case SceneValidationMode.AllScenes:
					return GetAllScenePathsInProject();

				case SceneValidationMode.AllBuildScenes:
					return GetAllScenePathsInBuildSettings();

				case SceneValidationMode.AllBuildAndAssetBundleScenes:
					var finalScenes = GetAllScenePathsInAssetBundles();
					finalScenes.AddRange(GetAllScenePathsInBuildSettings());
					return finalScenes;

				default:
					throw new ArgumentOutOfRangeException(Enum.GetName(typeof(SceneValidationMode), validationMode));
			}
		}

		/// <summary>
		/// Returns a list of scene paths in relative path format from the Unity Assets folder for all
		/// <see cref="SceneAsset"/>(s) in the project.
		/// </summary>
		/// <returns></returns>
		public static List<string> GetAllScenePathsInProject()
		{
			var scenePaths = new List<string>();

			var assetPaths = AssetDatabase.FindAssets(ProjectSearchFilterForScenes);
			for (var i = 0; i < assetPaths.Length; i++)
			{
				scenePaths.Add(AssetDatabase.GUIDToAssetPath(assetPaths[i]));
			}

			return scenePaths;
		}

		/// <summary>
		/// Returns a list of scene paths in relative path format from the Unity Assets folder for all
		/// <see cref="SceneAsset"/>(s) included in <see cref="AssetBundle"/>(s).
		/// </summary>
		/// <returns></returns>
		public static List<string> GetAllScenePathsInAssetBundles()
		{
			var sceneNames = new List<string>();
			var allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
			for (var i = 0; i < allAssetBundleNames.Length; i++)
			{
				var assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(allAssetBundleNames[i]);
				for (var j = 0; j < assetNames.Length; j++)
				{
					if (assetNames[j].Contains(SceneAssetExtension))
					{
						sceneNames.Add(assetNames[j]);
					}
				}
			}

			return sceneNames;
		}

		/// <summary>
		/// Returns a list of scene paths in relative path format from the Unity Assets folder for all
		/// <see cref="SceneAsset"/>(s) included in the build settings.
		/// </summary>
		/// <returns></returns>
		public static List<string> GetAllScenePathsInBuildSettings()
		{
			return EditorBuildSettings.scenes.Select(x => x.path).ToList();
		}

		/// <summary>
		/// Returns a human-readable string description for the passed <see cref="Type"/> <paramref name="type"/>
		/// based on its decorated <see cref="ValidatorDescriptionAttribute"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetValidatorDescription(Type type)
		{
			var vType = typeof(ValidatorDescriptionAttribute);
			var attrs = type.GetCustomAttributes(vType, false) as ValidatorDescriptionAttribute[];
			return attrs != null && attrs.Length > 0
				? attrs[0].Description
				: string.Empty;
		}

		/// <summary>
		/// Attempts to ping an <see cref="UnityEngine.Object"/> instance in the current scene or project window
		/// based on the passed <see cref="ValidationLog"/> <paramref name="log"/>.
		/// </summary>
		/// <param name="log"></param>
		internal static void TryPingObject(ValidationLog log)
		{
			UnityEngine.Object obj = null;
			if (log.source == LogSource.Scene)
			{
				obj = GameObject.Find(log.objectPath);
			}
			else if (log.source == LogSource.Project)
			{
				obj = AssetDatabase.LoadAssetAtPath(log.objectPath, typeof(UnityEngine.Object));
			}

			if (obj != null)
			{
				EditorGUIUtility.PingObject(obj);
			}
			else
			{
				Debug.LogWarningFormat(EditorConstants.CouldNotPingObjectWarning, log.objectPath);
			}
		}

		/// <summary>
		/// Returns true if the passed asset path includes a magic Unity Resources folder as a part of it,
		/// otherwise returns false.
		/// </summary>
		/// <param name="assetPath"></param>
		/// <returns></returns>
		public static bool IsInResourcesFolder(string assetPath)
		{
			var splitPath = assetPath.Split(EditorConstants.ForwardSlashChar);
			for (var i = 0; i < splitPath.Length; i++)
			{
				if (splitPath[i].ToLowerInvariant() == EditorConstants.ResourcesFolderName)
				{
					return true;
				}
			}

			return false;
		}
	}
}

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
using UnityEditor;
using UnityEngine;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// A custom inspector for <see cref="AssetValidatorOverrideConfig"/>
	/// </summary>
	[CustomEditor(typeof(AssetValidatorOverrideConfig))]
	internal sealed class AssetValidatorOverrideConfigInspector : UnityEditor.Editor
	{
		private AssetValidatorOverrideConfig _config;

		private void OnEnable()
		{
			_config = (AssetValidatorOverrideConfig)target;
			_config.FindAndAddMissingTypes();
		}

		public override void OnInspectorGUI()
		{
			var path = AssetDatabase.GetAssetPath(target);
			if (!ProjectTools.IsInResourcesFolder(path))
			{
				EditorGUILayout.HelpBox(
					EditorConstants.ConfigAssetNotInResourcesPath,
					MessageType.Warning);
			}

			var overrideItems = _config.OverrideItems;
			overrideItems.Sort(Comparison);

			var headerRect = EditorGUILayout.BeginVertical();
			headerRect.height += 10f;

			if (Event.current.type == EventType.Repaint)
			{
				var originalColor = GUI.color;
				GUI.color = new Color(0.5f, 0.5f, 0.5f, 1);
				GraphicsTools.LogRowHeaderBackground.Draw(headerRect, false, false, false, false);
				GUI.color = originalColor;
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			EditorGUILayout.LabelField(
				EditorConstants.OverrideConfigEnabledHeader,
				EditorStyles.boldLabel,
				GUILayout.Width(80f));
			EditorGUILayout.LabelField(
				EditorConstants.OverrideConfigClassHeader,
				EditorStyles.boldLabel,
				GUILayout.Width(200f));
			EditorGUILayout.LabelField(
				EditorConstants.OverrideConfigTypeHeader,
				EditorStyles.boldLabel,
				GUILayout.Width(200f));
			GUILayout.Space(20f);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();

			var contentsRect = EditorGUILayout.BeginVertical();
			GUILayout.Space(5f);
			GUI.Box(contentsRect, GraphicsTools.GrayTexture2D);
			EditorGUI.BeginChangeCheck();
			for (var i = 0; i < overrideItems.Count; i++)
			{
				var item = overrideItems[i];

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(20f);
				item.enabled = EditorGUILayout.Toggle(item.enabled, GUILayout.Width(80f));
				EditorGUILayout.LabelField(item.type.Name, GUILayout.Width(200f));
				EditorGUILayout.LabelField(GetTypeOfValidator(item.type), GUILayout.Width(200f));
				GUILayout.Space(20f);
				EditorGUILayout.EndHorizontal();
			}

			var valueHasChanged = EditorGUI.EndChangeCheck();
			GUILayout.Space(5f);
			EditorGUILayout.EndVertical();

			if (!valueHasChanged)
			{
				return;
			}

			Repaint();
			EditorUtility.SetDirty(_config);
		}

		/// <summary>
		/// Compare and sort <see cref="AssetValidatorOverrideConfig.OverrideItem"/>s based on the type of
		/// validator they are and if the same validator type, the alphabetical spelling of their type name.
		/// </summary>
		/// <param name="itemOne"></param>
		/// <param name="itemTwo"></param>
		/// <returns></returns>
		private static int Comparison(
			AssetValidatorOverrideConfig.OverrideItem itemOne,
			AssetValidatorOverrideConfig.OverrideItem itemTwo)
		{
			var typeOne = GetTypeOfValidator(itemOne.type);
			var typeTwo = GetTypeOfValidator(itemTwo.type);

			return typeOne != typeTwo
				? string.Compare(typeOne, typeTwo, StringComparison.Ordinal)
				: string.Compare(itemOne.type.Name, itemTwo.type.Name, StringComparison.Ordinal);
		}

		/// <summary>
		/// Returns a human-readable label describing the type of validator <see cref="Type"/>
		/// <paramref name="type"/> is based on which abstract validator type it is derived from (if any).
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static string GetTypeOfValidator(Type type)
		{
			if (type.IsSubclassOf(typeof(ObjectValidatorBase)))
			{
				return EditorConstants.ObjectValidatorTypeName;
			}

			if (type.IsSubclassOf(typeof(FieldValidatorBase)))
			{
				return EditorConstants.FieldValidatorTypeName;
			}

			if (type.IsSubclassOf(typeof(CrossSceneValidatorBase)))
			{
				return EditorConstants.CrossSceneValidatorTypeName;
			}

			if (type.IsSubclassOf(typeof(ProjectValidatorBase)))
			{
				return EditorConstants.ProjectValidatorTypeName;
			}

			return EditorConstants.UnknownValidatorTypeName;
		}
	}
}

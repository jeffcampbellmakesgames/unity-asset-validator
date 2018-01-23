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
using JCMG.AssetValidator.Editor.Utility;
using JCMG.AssetValidator.Editor.Validators.CrossSceneValidators;
using JCMG.AssetValidator.Editor.Validators.FieldValidators;
using JCMG.AssetValidator.Editor.Validators.ObjectValidators;
using JCMG.AssetValidator.Editor.Validators.ProjectValidators;
using System;
using UnityEditor;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Config
{
    [CustomEditor(typeof(AssetValidatorOverrideConfig))]
    public class AssetValidatorOverrideConfigInspector : UnityEditor.Editor
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
            if(!IsInResourcesFolder(path))
            {
                EditorGUILayout.HelpBox("This asset must be located at the root of a Resources folder in order for " +
                                        "it to be loadable and used by the AssetValidator. Otherwise it will be ignored.", MessageType.Warning);
            }            

            var oItems = _config.OverrideItems;
            oItems.Sort(Comparison);

            var headerRect = EditorGUILayout.BeginVertical();
            headerRect.height += 10f;

#if UNITY_2017_3_OR_NEWER
            if (Event.current.type == EventType.Repaint)
#else
            if (Event.current.type == EventType.repaint)
#endif
            {
                var originalColor = GUI.color;
                GUI.color = new Color(0.5f, 0.5f, 0.5f, 1);
                AssetValidatorGraphicsUtility.Styles.HeaderBackground.Draw(headerRect, false, false, false, false);
                GUI.color = originalColor;
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            EditorGUILayout.LabelField("Is Enabled", EditorStyles.whiteBoldLabel, GUILayout.Width(80f));
            EditorGUILayout.LabelField("Validator Class", EditorStyles.whiteBoldLabel, GUILayout.Width(200f));
            EditorGUILayout.LabelField("Validator Type", EditorStyles.whiteBoldLabel, GUILayout.Width(200f));
            GUILayout.Space(20f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            var contentsRect = EditorGUILayout.BeginVertical();
            GUILayout.Space(5f);
            GUI.Box(contentsRect, AssetValidatorGraphicsUtility.GrayTexture2D);
            EditorGUI.BeginChangeCheck();
            for (var i = 0; i < oItems.Count; i++)
            {
                var item = oItems[i];

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

            if (!valueHasChanged) return;

            Repaint();
            EditorUtility.SetDirty(_config);
        }

        private bool IsInResourcesFolder(string assetPath)
        {
            var splitPath = assetPath.Split('/');

            for (var i = 0; i < splitPath.Length; i++)
                if (splitPath[i] == "Resources")
                    return true;

            return false;
        }

        private int Comparison(AssetValidatorOverrideConfig.OverrideItem itemOne, AssetValidatorOverrideConfig.OverrideItem itemTwo)
        {
            var typeOne = GetTypeOfValidator(itemOne.type);
            var typeTwo = GetTypeOfValidator(itemTwo.type);

            return typeOne != typeTwo ? typeOne.CompareTo(typeTwo) : itemOne.type.Name.CompareTo(itemTwo.type.Name);
        }

        private string GetTypeOfValidator(Type type)
        {
            if (type.IsSubclassOf(typeof(BaseObjectValidator))) return "Object Validator";
            if (type.IsSubclassOf(typeof(BaseFieldValidator))) return "Field Validator";
            if (type.IsSubclassOf(typeof(BaseCrossSceneValidator))) return "Cross Scene Validator";
            if (type.IsSubclassOf(typeof(BaseProjectValidator))) return "Project Validator";

            return "Unknown";
        }
    }
}
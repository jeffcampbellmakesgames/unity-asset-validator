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
using UnityEditor;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Validators.FieldValidators.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(NonNullAttribute))]
    public class VNonNullPropertyDrawer : PropertyDrawer
    {
        private bool isInvalid;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetObject = property.serializedObject.targetObject as object;
            var targetObjectClassType = targetObject.GetType();
            var field = targetObjectClassType.GetField(property.propertyPath);
            var value = field.GetValue(targetObject);

            isInvalid = value == null || value.ToString() == "null";
            if (isInvalid)
            {
                label = EditorGUI.BeginProperty(position, label, property);
                var contentPosition = EditorGUI.PrefixLabel(position, label, AssetValidatorGraphicsUtility.ErrorStyle);
                EditorGUI.indentLevel = 0;
                EditorGUI.PropertyField(contentPosition, property, GUIContent.none);
                EditorGUI.EndProperty();

                //EditorGUI.LabelField(new Rect(position.x, position.y, position.width, position.height / 2f), "Missing Ref!", AssetValidatorGraphicsUtility.ErrorStyle);
                //EditorGUI.PropertyField(new Rect(position.x, position.y + position.height, position.width, position.height / 2f), property);
            }
            else
            {
                EditorGUI.PropertyField(position, property);
            }
        }
    }
}

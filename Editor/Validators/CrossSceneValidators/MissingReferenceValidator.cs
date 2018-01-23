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
using JCMG.AssetValidator.Editor.Validators.Output;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Validators.CrossSceneValidators
{
    [ValidatorTarget("cross_scene_missing_component_reference")]
    [ValidatorDescription("Checks one or more Scenes to see if there are any missing Component references on every Gameobject " +
                          "and and their SerializedProperties in that scene.")]
    public class MissingReferenceValidator : BaseCrossSceneValidator
    {
        private bool _foundMissingReferenceComponent;

        private const string _missingReferenceComponentError = "There is a missing component on Gameobject [{0}]";
        private const string _missingReferencePropertyError = "There is a missing component reference on Gameobject [{0}] on component [{1}] for property [{2}].";

        public override void Search()
        {
            var objects = Object.FindObjectsOfType<GameObject>();
            var currentScene = EditorSceneManager.GetActiveScene().path;

            foreach (var obj in objects)
            {
                var components = obj.GetComponents<Component>();

                foreach (var c in components)
                {
                    if (!c)
                    {
                        DispatchVLogEvent(obj,
                                          VLogType.Error,
                                          string.Format(_missingReferenceComponentError, obj.name),
                                          currentScene);

                        _foundMissingReferenceComponent = true;
                    }
                    else
                    {
                        var so = new SerializedObject(c);
                        var sp = so.GetIterator();

                        while (sp.NextVisible(true))
                        {
                            if (sp.propertyType != SerializedPropertyType.ObjectReference) continue;

                            if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0)
                            {
                                DispatchVLogEvent(obj,
                                                  VLogType.Error,
                                                  string.Format(_missingReferencePropertyError,
                                                                obj.name,
                                                                c.GetType().Name,
                                                                ObjectNames.NicifyVariableName(sp.name)),
                                                  currentScene);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Return true only if we did not find any missing 
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            if (_foundMissingReferenceComponent)
            {
                DispatchVLogEvent(new VLog()
                {
                    vLogType = VLogType.Error,
                    source = VLogSource.None,
                    validatorName = TypeName,
                    scenePath = string.Empty,
                    objectPath = string.Empty,
                    message = "Missing component references were found in the Scene(s) searched..."
                });
            }

            return !_foundMissingReferenceComponent;
        }
    }
}
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
using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JCMG.AssetValidator.Editor.Validators.CrossSceneValidators
{
    /// <summary>
    /// This validator should be derived from where there should only ever be one instance of a 
    /// component present in the scenes searched.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ValidatorDescription("This validator should be derived from where there should only ever be " +
                          "one instance of a component present in the scenes searched.")]
    [ValidatorExample(@"
/// <summary>
/// FooComponent is a Monobehavior derived component. There should only be 
/// one FooComponent for any of the scenes searched.
/// </summary>
public class FooComponent : MonoBehaviour
{

}

/// <summary>
/// This is a subclass of EnsureComponentIsUniqueValidator; as the generic type is of FooComponent,  
/// when this validator searches across one or more scenes, if it finds more than one instance of 
/// FooComponent a validation warning will be dispatched per instance and an error noting that there 
/// should only be one instance.
/// </summary>
public class EnsureFooComponentIsUniqueValidator : EnsureComponentIsUniqueValidator<FooComponent>
{
        
}
")]
    public class EnsureComponentIsUniqueValidator<T> : BaseCrossSceneValidator 
        where T : Component
    {
        private readonly List<VLog> _vLogList = new List<VLog>();
        private readonly Type _typeToTrack;

        public virtual string UniqueTarget
        {
            get { return _typeToTrack.Name; }
        }

        public EnsureComponentIsUniqueValidator()
        {
            _typeToTrack = typeof(T);
        }

        public override void Search()
        {
            var scenePath = EditorSceneManager.GetActiveScene().path;
            var component = Object.FindObjectsOfType(_typeToTrack);

            // Create a warning log for each event system found. These are 
            // only used if more than one is found across all scenes
            for (var i = 0; i < component.Length; i++)
                if(ShouldAddComponent((T)component[i]))
                    _vLogList.Add(CreateVLog(component[i], 
                                  VLogType.Warning,
                                  string.Format("Scene at path [{0}] is not the only Scene to contain an [{1}]...", 
                                                EditorSceneManager.GetActiveScene().path, UniqueTarget),
                                  scenePath));
        }

        public virtual bool ShouldAddComponent(T component)
        {
            return true;
        }

        public override bool Validate()
        {
            if (_vLogList.Count <= 1) return true;

            for (var i = 0; i < _vLogList.Count; i++)
                DispatchVLogEvent(_vLogList[i]);

            DispatchVLogEvent(new VLog()
            {
                vLogType = VLogType.Error,
                source = VLogSource.None,
                validatorName = TypeName,
                scenePath = string.Empty,
                objectPath = string.Empty,
                message = string.Format("More than one Scene of the Scene(s) validated has an [{0}] present", _typeToTrack.Name)
            });

            return false;
        }
    }
}

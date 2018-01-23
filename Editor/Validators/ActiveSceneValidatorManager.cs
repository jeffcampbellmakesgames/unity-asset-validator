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
using JCMG.AssetValidator.Editor.Meta;
using JCMG.AssetValidator.Editor.Validators.Output;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Validators
{
    /// <summary>
    /// A ActiveSceneValidator inspects components in a single scene whose type 
    /// is contained in the passed ClassTypeCache
    /// </summary>
    public class ActiveSceneValidatorManager : BaseInstanceValidatorManager
    {
        public ActiveSceneValidatorManager(ClassTypeCache cache, AssetValidatorLogger logger) 
            : base(cache, logger)
        {
        }

        public override void Search()
        {
            _objectsToValidate.Clear();
            for (var i = 0; i < _cache.Count; i++)
                _objectsToValidate.AddRange(Object.FindObjectsOfType(_cache[i]));
        }

        protected override void OnLogEvent(VLog vLog)
        {
            vLog.scenePath = EditorSceneManager.GetActiveScene().path;
            vLog.source = VLogSource.Scene;

            base.OnLogEvent(vLog);
        }
    }
}

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
using JCMG.AssetValidator.Editor.Validators.Output;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JCMG.AssetValidator.Editor.Validators.CrossSceneValidators
{
    public class BaseCrossSceneValidator
    {
        public Action<VLog> OnLogEvent;

        private string _typeName;
        public string TypeName
        {
            get
            {
                return string.IsNullOrEmpty(_typeName) 
                    ? (_typeName = GetType().Name) 
                    : _typeName;
            }
        }

        /// <summary>
        /// Search the current Scene and add the information gathered to a cache so that
        /// we can validate it in aggregate after the target Scene(s) have been searched.
        /// </summary>
        public virtual void Search()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// After all scenes have been searched, validate using the aggregated results.
        /// </summary>
        /// <returns></returns>
        public virtual bool Validate()
        {
            throw new NotImplementedException();
        }

        protected VLog CreateVLog(Object obj, 
                                  VLogType type, 
                                  string message, 
                                  string scenePath = "", 
                                  VLogSource source = VLogSource.Scene)
        {
            return new VLog()
            {
                vLogType = type,
                source = source,
                validatorName = TypeName,
                message = message,
                objectPath = ObjectUtility.GetObjectPath(obj),
                scenePath = scenePath
            };
        }

        protected void DispatchVLogEvent(VLog vlog)
        {
            if (OnLogEvent != null)
                OnLogEvent(vlog);
        }

        protected void DispatchVLogEvent(Object obj,
            VLogType type,
            string message,
            string scenePath = "",
            VLogSource source = VLogSource.Scene)
        {
            if (OnLogEvent != null)
                OnLogEvent(new VLog()
                {
                    vLogType = type,
                    source = source,
                    validatorName = TypeName,
                    message = message,
                    objectPath = ObjectUtility.GetObjectPath(obj),
                    scenePath = scenePath
                });
        }
    }
}

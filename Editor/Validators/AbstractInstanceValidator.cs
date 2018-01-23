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
using Object = UnityEngine.Object;

namespace JCMG.AssetValidator.Editor.Validators
{
    public abstract class AbstractInstanceValidator
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

        protected Type _typeToTrack;

        public abstract bool Validate(Object o);

        public abstract bool AppliesTo(Object obj);

        public abstract bool AppliesTo(Type type);

        protected void DispatchVLogEvent(Object obj, VLogType type, string message)
        {
            if (OnLogEvent != null)
            {
                OnLogEvent(new VLog()
                {
                    vLogType = type,
                    validatorName = TypeName,
                    message = message,
                    objectPath = ObjectUtility.GetObjectPath(obj)
                });
            }
        }
    }
}
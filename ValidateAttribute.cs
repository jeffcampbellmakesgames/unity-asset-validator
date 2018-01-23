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
using System;
using System.Diagnostics;

namespace JCMG.AssetValidator
{
    /// <summary>
    /// [Validate] is used by the validator editor tools to determine which Monobehaviour 
    /// or ScriptableObject classes should be targeted for validation. It is necessary when 
    /// using Field Validation Attributes that [Validate] is on the class whose Fields are 
    /// being decorated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class), Conditional("UNITY_EDITOR")]
    public class ValidateAttribute : Attribute
    {
        public UnityTarget UnityTarget { get; private set; }

        public ValidateAttribute()
        {
            UnityTarget = UnityTarget.Monobehavior;
        }

        protected ValidateAttribute(UnityTarget unityTarget)
        {
            UnityTarget = unityTarget;
        }
    }
}

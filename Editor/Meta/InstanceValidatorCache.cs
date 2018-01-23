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
using JCMG.AssetValidator.Editor.Validators;
using JCMG.AssetValidator.Editor.Validators.FieldValidators;
using JCMG.AssetValidator.Editor.Validators.ObjectValidators;
using System.Collections.Generic;
using System.Linq;
using JCMG.AssetValidator.Editor.Config;

namespace JCMG.AssetValidator.Editor.Meta
{
    /// <summary>
    /// InstanceValidatorCache is a wrapper around all validator types that target individual
    /// class or object instances.
    /// </summary>
    public class InstanceValidatorCache
    {
        private readonly List<AbstractInstanceValidator> _validators;
        
        public InstanceValidatorCache()
        {
            _validators = new List<AbstractInstanceValidator>();

            // Make sure any overriden disabled types are not included
            var overrideConfig = AssetValidatorOverrideConfig.FindOrCreate();

            // Get and add all field validators, excluding override disabled ones.
            var fieldValidators = ReflectionUtility.GetAllDerivedInstancesOfType<BaseFieldValidator>().ToArray();
            for (var index = 0; index < fieldValidators.Length; index++)
            {
                AssetValidatorOverrideConfig.OverrideItem item = null;
                var vFieldValidator = fieldValidators[index];
                if (overrideConfig.TryGetOverrideConfigItem(vFieldValidator.GetType(), out item))
                {
                    if(item.enabled)
                        _validators.Add(vFieldValidator);
                }
                else
                    _validators.Add(vFieldValidator);
            }

            // Get and add all object validators, excluding override disabled ones.
            var objectValidators = ReflectionUtility.GetAllDerivedInstancesOfType<BaseObjectValidator>().ToArray();
            for (var index = 0; index < objectValidators.Length; index++)
            {
                AssetValidatorOverrideConfig.OverrideItem item = null;
                var vObjectValidator = objectValidators[index];
                if (overrideConfig.TryGetOverrideConfigItem(vObjectValidator.GetType(), out item))
                {
                    if (item.enabled)
                        _validators.Add(vObjectValidator);
                }
                else
                    _validators.Add(vObjectValidator);
            }
        }

        public int Count
        {
            get { return _validators.Count; }
        }

        public AbstractInstanceValidator this[int index]
        {
            get { return _validators[index]; }
        }
    }
}

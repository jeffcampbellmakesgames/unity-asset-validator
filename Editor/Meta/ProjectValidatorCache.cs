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
using JCMG.AssetValidator.Editor.Config;
using JCMG.AssetValidator.Editor.Utility;
using JCMG.AssetValidator.Editor.Validators;
using JCMG.AssetValidator.Editor.Validators.ProjectValidators;
using System.Collections.Generic;
using System.Linq;

namespace JCMG.AssetValidator.Editor.Meta
{
    /// <summary>
    /// ProjectValidatorCache is a wrapper around all validator types that exclusively target
    /// objects and assets in the project.
    /// </summary>
    public class ProjectValidatorCache
    {
        private List<BaseProjectValidator> _validators;

        public ProjectValidatorCache()
        {
            _validators = new List<BaseProjectValidator>();

            // Make sure any overriden disabled types are not included
            var overrideConfig = AssetValidatorOverrideConfig.FindOrCreate();

            // Get and add all field validators, excluding override disabled ones.
            var pvs = ReflectionUtility.GetAllDerivedInstancesOfTypeWithAttribute<BaseProjectValidator, ValidatorTargetAttribute>().ToArray();
            for (var index = 0; index < pvs.Length; index++)
            {
                AssetValidatorOverrideConfig.OverrideItem item = null;
                var projectValidator = pvs[index];
                if (overrideConfig.TryGetOverrideConfigItem(projectValidator.GetType(), out item))
                {
                    if (item.enabled)
                        _validators.Add(projectValidator);
                }
                else
                    _validators.Add(projectValidator);
            }
        }

        public int Count
        {
            get { return _validators.Count; }
        }

        public BaseProjectValidator this[int index]
        {
            get { return _validators[index]; }
        }
    }
}
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
using JCMG.AssetValidator.Editor.Validators.CrossSceneValidators;
using System.Collections.Generic;
using System.Linq;

namespace JCMG.AssetValidator.Editor.Meta
{
    public class CrossSceneValidatorCache
    {
        private readonly List<BaseCrossSceneValidator> _validators;

        public CrossSceneValidatorCache()
        {
            var validators = ReflectionUtility.GetAllDerivedInstancesOfType<BaseCrossSceneValidator>().ToArray();
            _validators = new List<BaseCrossSceneValidator>();

            // Make sure any overriden disabled types are not included
            var overrideConfig = AssetValidatorOverrideConfig.FindOrCreate();
            for (var index = 0; index < validators.Length; index++)
            {
                AssetValidatorOverrideConfig.OverrideItem item = null;
                var baseCrossSceneValidator = validators[index];
                if (overrideConfig.TryGetOverrideConfigItem(baseCrossSceneValidator.GetType(), out item))
                {
                    if (item.enabled)
                        _validators.Add(baseCrossSceneValidator);
                }
                else
                    _validators.Add(baseCrossSceneValidator);
            }
        }

        public int Count
        {
            get { return _validators.Count; }
        }

        public BaseCrossSceneValidator this[int index]
        {
            get { return _validators[index]; }
        }
    }
}

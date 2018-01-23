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
using JCMG.AssetValidator.Editor.Validators;
using JCMG.AssetValidator.Editor.Validators.CrossSceneValidators;
using JCMG.AssetValidator.Editor.Validators.FieldValidators;
using JCMG.AssetValidator.Editor.Validators.ObjectValidators;
using JCMG.AssetValidator.Editor.Validators.Output;
using JCMG.AssetValidator.Editor.Validators.ProjectValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Config
{
    public class AssetValidatorOverrideConfig : ScriptableObject
    {
        public List<OverrideItem> OverrideItems = new List<OverrideItem>();

        private const string ASSET_PATH = "ValidatorOverrideConfig";

        [Serializable]
        public class OverrideItem
        {
            public bool enabled;
            public string symbol;

            [NonSerialized]
            public Type type;

            public override string ToString()
            {
                return string.Format("Validator with Symbol [{0}] is Enabled: [{1}]", symbol, enabled);
            }
        }

        private static AssetValidatorOverrideConfig _config;
        public static AssetValidatorOverrideConfig FindOrCreate()
        {
            if(_config == null)
                _config = Resources.Load<AssetValidatorOverrideConfig>(ASSET_PATH);

            if(_config != null)
            {
                _config.FindAndAddMissingTypes();
                return _config;
            }

            Debug.LogWarningFormat("Could not find an AssetValidatorOverrideConfig named [{0}] at the root of a Resources folder. " +
                                   "Overriding will be ignored in its absence.", ASSET_PATH);

            return CreateInstance<AssetValidatorOverrideConfig>();            
        }

        private static AssetValidatorOverrideConfig CreateAsset()
        {
            var asset = CreateInstance(typeof(AssetValidatorOverrideConfig));
            AssetDatabase.CreateAsset(asset, ASSET_PATH);
            AssetDatabase.SaveAssets();

            return asset as AssetValidatorOverrideConfig;
        }

        public void FindAndAddMissingTypes()
        {
            var classCache = new ClassTypeCache();
            classCache.IgnoreAttribute<OnlyIncludeInTestsAttribute>();
            classCache.AddTypeWithAttribute<BaseFieldValidator, ValidatorTargetAttribute>();
            classCache.AddTypeWithAttribute<BaseObjectValidator, ValidatorTargetAttribute>();
            classCache.AddTypeWithAttribute<BaseCrossSceneValidator, ValidatorTargetAttribute>();
            classCache.AddTypeWithAttribute<BaseProjectValidator, ValidatorTargetAttribute>();

            var validatorTargets = classCache.Types.Select(x =>
            {
                var vValidatorAttr = (ValidatorTargetAttribute)x.GetCustomAttributes(typeof(ValidatorTargetAttribute), false)[0];
                return vValidatorAttr;
            }).ToArray();

            if (OverrideItems == null)
                OverrideItems = new List<OverrideItem>();

            // Remove any missing override items that no longer exist
            for (var i = OverrideItems.Count - 1; i > 0; i--)
            {
                if (validatorTargets.Any(x => x.Symbol == OverrideItems[i].symbol)) continue;

                OverrideItems.Remove(OverrideItems[i]);
            }

            for (var i = 0; i < validatorTargets.Length; i++)
            {
                var vValidatorAttr = validatorTargets[i];

                // If we have never cached this type before, create a reference to it by way of symbol
                // Otherwise grab the existing reference and reassign the type.
                if (OverrideItems.All(x => x.symbol != vValidatorAttr.Symbol))
                {
                    var oItem = new OverrideItem()
                    {
                        enabled = true,
                        symbol = vValidatorAttr.Symbol,
                        type = classCache[i]
                    };
                    OverrideItems.Add(oItem);
                }
                else
                {
                    var overrideItem = OverrideItems.First(x => x.symbol == vValidatorAttr.Symbol);
                    overrideItem.type = classCache[i];
                }
            }
        }

        public bool TryGetOverrideConfigItem(Type type, out OverrideItem overrideItem)
        {
            overrideItem = null;

            for (var i = 0; i < OverrideItems.Count; i++)
                if (OverrideItems[i].type == type)
                {
                    overrideItem = OverrideItems[i];
                    return true;
                }

            return false;
        }

        public void AddDisabledLogs(AssetValidatorLogger logger)
        {
            for (var i = 0; i < OverrideItems.Count; i++)
            {
                if (OverrideItems[i].enabled) continue;

                logger.OnLogEvent(new VLog()
                {
                    source = VLogSource.None,
                    vLogType = VLogType.Warning,
                    message = string.Format("Validator of type [{0}] is disabled in the AssetValidatorOverrideConfig at [{1}]",
                                            OverrideItems[i].type.Name, AssetDatabase.GetAssetPath(this))
                });
            }
        }
    }
}

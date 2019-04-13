/*
MIT License

Copyright (c) 2019 Jeff Campbell

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
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// The <see cref="AssetValidatorOverrideConfig"/> allows for specifying which validators should be
	/// enabled or disabled at the project level in a configuration asset that can be checked into the project.
	/// By default, this is expected to be created in a resources folder for easy loading.
	/// </summary>
	[CreateAssetMenu(fileName = "AssetValidatorOverrideConfig",
		menuName = "JCMG/AssetValidator/AssetValidatorOverrideConfig")]
	internal class AssetValidatorOverrideConfig : ScriptableObject
	{
		private const string ZeroValidatorAttributesFoundWarning =
			"There should be one ValidatorAttribute on each validator implementation," +
			"[{0}] appears to have zero.";

		private const string MultipleValidatorAttributesFoundWarning =
			"There should be one ValidatorAttribute on each validator implementation," +
			"[{0}] appears to have more than one.";

		internal List<OverrideItem> OverrideItems
		{
			get { return _overrideItems; }
		}

		[SerializeField]
		private List<OverrideItem> _overrideItems;

		/// <summary>
		/// This is an individual instance of validator configuration
		/// </summary>
		[Serializable]
		internal class OverrideItem
		{
			/// <summary>
			/// Whether or not the validator is enabled.
			/// </summary>
			public bool enabled;

			/// <summary>
			/// A symbolic identifier for the validator gleaned from its
			/// <see cref="ValidatorAttribute"/>.
			/// </summary>
			public string symbol;

			/// <summary>
			/// The Type of the validator this <see cref="OverrideItem"/> applies to.
			/// </summary>
			[NonSerialized]
			public Type type;

			public override string ToString()
			{
				return string.Format(EditorConstants.OverrideConfigToString, symbol, enabled);
			}
		}

		private static AssetValidatorOverrideConfig _config;

		/// <summary>
		/// Find an existing <see cref="AssetValidatorOverrideConfig"/> in memory or project and where none
		/// exist create one in the project.
		/// </summary>
		/// <returns></returns>
		internal static AssetValidatorOverrideConfig FindOrCreate()
		{
			if (_config == null)
			{
				_config = Resources.Load<AssetValidatorOverrideConfig>(EditorConstants.OverrideConfigName);
			}

			if (_config != null)
			{
				_config.FindAndAddMissingTypes();
				return _config;
			}

			Debug.LogWarningFormat(EditorConstants.CouldNotFindConfigWarning);

			return CreateInstance<AssetValidatorOverrideConfig>();
		}

		/// <summary>
		/// Iterate through all validator types and ensure an <see cref="OverrideItem"/> exists for each one.
		/// For any that no longer exist, remove these.
		/// </summary>
		internal void FindAndAddMissingTypes()
		{
			var classCache = new ClassTypeCache();
			classCache.IgnoreAttribute<OnlyIncludeInTestsAttribute>();
			classCache.AddTypeWithAttribute<FieldValidatorBase, ValidatorAttribute>();
			classCache.AddTypeWithAttribute<ObjectValidatorBase, ValidatorAttribute>();
			classCache.AddTypeWithAttribute<CrossSceneValidatorBase, ValidatorAttribute>();
			classCache.AddTypeWithAttribute<ProjectValidatorBase, ValidatorAttribute>();

			var validatorTargets = new List<ValidatorAttribute>();
			for (var i = 0; i < classCache.Count; i++)
			{
				var type = classCache[i];
				var validatorAttrs = type.GetCustomAttributes(
						typeof(ValidatorAttribute),
						false)
					as ValidatorAttribute[];

				if (validatorAttrs == null || validatorAttrs.Length == 0)
				{
					Debug.LogWarningFormat(ZeroValidatorAttributesFoundWarning, type.FullName);
				}
				else if (validatorAttrs.Length > 1)
				{
					Debug.LogWarningFormat(MultipleValidatorAttributesFoundWarning, type.FullName);
				}
				else
				{
					validatorTargets.Add(validatorAttrs[0]);
				}
			}

			// Remove any missing override items that no longer exist
			for (var i = OverrideItems.Count - 1; i > 0; i--)
			{
				if (validatorTargets.Any(x => x.Symbol == OverrideItems[i].symbol))
				{
					continue;
				}

				OverrideItems.Remove(OverrideItems[i]);
			}

			for (var i = 0; i < validatorTargets.Count; i++)
			{
				var vValidatorAttr = validatorTargets[i];

				// If we have never cached this type before, create a reference to it by way of symbol
				// Otherwise grab the existing reference and reassign the type.
				if (OverrideItems.All(x => x.symbol != vValidatorAttr.Symbol))
				{
					var oItem = new OverrideItem
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

		/// <summary>
		/// Try and get an <see cref="OverrideItem"/> for validator <see cref="Type"/> <paramref name="type"/>.
		/// Returns true if found, otherwise false.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="overrideItem"></param>
		/// <returns></returns>
		internal bool TryGetOverrideConfigItem(Type type, out OverrideItem overrideItem)
		{
			overrideItem = null;
			for (var i = 0; i < OverrideItems.Count; i++)
			{
				if (OverrideItems[i].type != type)
				{
					continue;
				}

				overrideItem = OverrideItems[i];
				return true;
			}

			return false;
		}

		/// <summary>
		/// Adds to an <see cref="LogCache"/> <paramref name="_logCache"/> logs for all of the
		/// validators that have been disabled for this run.
		/// </summary>
		/// <param name="_logCache"></param>
		internal void AddDisabledLogs(LogCache _logCache)
		{
			for (var i = 0; i < OverrideItems.Count; i++)
			{
				if (OverrideItems[i].enabled)
				{
					continue;
				}

				_logCache.OnLogCreated(new ValidationLog
				{
					source = LogSource.None,
					logType = LogType.Warning,
					message = string.Format(
						EditorConstants.ConfigValidatorDisabledWarning,
						OverrideItems[i].type.Name,
						AssetDatabase.GetAssetPath(this))
				});
			}
		}

		/// <summary>
		/// Resets the internal configuration for the <see cref="AssetValidatorOverrideConfig"/> to its
		/// starting value defaults.
		/// </summary>
		private void Reset()
		{
			_overrideItems = new List<OverrideItem>();
		}
	}
}

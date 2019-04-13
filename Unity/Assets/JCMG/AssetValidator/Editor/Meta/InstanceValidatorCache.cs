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
using System.Collections.Generic;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="InstanceValidatorCache"/> is a cache of <see cref="AbstractInstanceValidator"/>
	/// derived instance(s).
	/// </summary>
	internal class InstanceValidatorCache
	{
		private readonly List<AbstractInstanceValidator> _validators;

		/// <summary>
		/// Constructor to create a pre-filled <see cref="InstanceValidatorCache"/>.
		/// </summary>
		public InstanceValidatorCache()
		{
			_validators = new List<AbstractInstanceValidator>();

			// Make sure any overriden disabled types are not included
			var overrideConfig = AssetValidatorOverrideConfig.FindOrCreate();

			// Get and add all field validators, excluding override disabled ones.
			var fieldValidators = ReflectionTools.GetAllDerivedInstancesOfType<FieldValidatorBase>();
			foreach (var fieldValidator in fieldValidators)
			{
				var t = fieldValidator.GetType();

				AssetValidatorOverrideConfig.OverrideItem overrideItem;
				if (overrideConfig.TryGetOverrideConfigItem(t, out overrideItem) && overrideItem.enabled)
				{
					_validators.Add(fieldValidator);
				}
			}

			// Get and add all object validators, excluding override disabled ones.
			var objectValidators = ReflectionTools.GetAllDerivedInstancesOfType<ObjectValidatorBase>();
			foreach (var objectValidator in objectValidators)
			{
				var t = objectValidator.GetType();

				AssetValidatorOverrideConfig.OverrideItem overrideItem;
				if (overrideConfig.TryGetOverrideConfigItem(t, out overrideItem) && overrideItem.enabled)
				{
					_validators.Add(objectValidator);
				}
			}
		}

		/// <summary>
		/// The total count of <see cref="AbstractInstanceValidator"/> derived instance(s) in the cache.
		/// </summary>
		public int Count
		{
			get { return _validators.Count; }
		}

		/// <summary>
		/// Returns the <see cref="AbstractInstanceValidator"/> derived instance at position
		/// <paramref name="index"/> in the cache.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public AbstractInstanceValidator this[int index]
		{
			get { return _validators[index]; }
		}
	}
}

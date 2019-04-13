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
	/// <see cref="CrossSceneValidatorCache"/> is a cache of <see cref="CrossSceneValidatorBase"/> derived
	/// instance(s).
	/// </summary>
	internal class CrossSceneValidatorCache
	{
		private readonly List<CrossSceneValidatorBase> _validators;

		/// <summary>
		/// Constructor to create a pre-filled <see cref="CrossSceneValidatorCache"/>.
		/// </summary>
		public CrossSceneValidatorCache()
		{
			// Find all non-abstract derived instances of CrossSceneValidatorBase
			var validators = ReflectionTools.GetAllDerivedInstancesOfType<CrossSceneValidatorBase>();
			_validators = new List<CrossSceneValidatorBase>();

			// Make sure any overriden, disabled validator types are not included
			var overrideConfig = AssetValidatorOverrideConfig.FindOrCreate();
			foreach (var baseCrossSceneValidator in validators)
			{
				var t = baseCrossSceneValidator.GetType();

				AssetValidatorOverrideConfig.OverrideItem overrideItem;
				if (overrideConfig.TryGetOverrideConfigItem(t, out overrideItem) && overrideItem.enabled)
				{
					_validators.Add(baseCrossSceneValidator);
				}
			}
		}

		/// <summary>
		/// The total count of <see cref="CrossSceneValidatorBase"/> derived instance(s) in the cache.
		/// </summary>
		public int Count
		{
			get { return _validators.Count; }
		}

		/// <summary>
		/// Returns a <see cref="CrossSceneValidatorBase"/> derived instance at position
		/// <paramref name="index"/> in the cache.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public CrossSceneValidatorBase this[int index]
		{
			get { return _validators[index]; }
		}
	}
}

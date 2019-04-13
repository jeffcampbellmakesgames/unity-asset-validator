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
	/// <see cref="ProjectValidatorCache"/> is a cache of <see cref="ProjectValidatorBase"/> derived instances.
	/// </summary>
	internal class ProjectValidatorCache
	{
		private readonly List<ProjectValidatorBase> _validators;

		/// <summary>
		/// Constructor to create a pre-filled <see cref="ProjectValidatorCache"/>.
		/// </summary>
		public ProjectValidatorCache()
		{
			_validators = new List<ProjectValidatorBase>();

			// Make sure any overriden disabled types are not included
			var overrideConfig = AssetValidatorOverrideConfig.FindOrCreate();

			// Get and add all field validators, excluding override disabled ones.
			var pvs = ReflectionTools.GetAllDerivedInstancesOfTypeWithAttribute<ProjectValidatorBase, ValidatorAttribute>();
			foreach (var projectValidator in pvs)
			{
				var t = projectValidator.GetType();

				AssetValidatorOverrideConfig.OverrideItem overrideItem;
				if (overrideConfig.TryGetOverrideConfigItem(t, out overrideItem) && overrideItem.enabled)
				{
					_validators.Add(projectValidator);
				}
			}
		}

		/// <summary>
		/// The total count of <see cref="ProjectValidatorBase"/> derived instances in the cache.
		/// </summary>
		public int Count
		{
			get { return _validators.Count; }
		}

		/// <summary>
		/// Returns the <see cref="ProjectValidatorBase"/> derived instance at position
		/// <paramref name="index"/> in the cache.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ProjectValidatorBase this[int index]
		{
			get { return _validators[index]; }
		}
	}
}

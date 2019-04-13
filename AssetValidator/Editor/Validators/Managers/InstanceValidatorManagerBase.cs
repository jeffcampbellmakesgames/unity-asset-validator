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
using Object = UnityEngine.Object;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="InstanceValidatorManagerBase"/> is the abstract base class for all validator managers that
	/// inspect object instances.
	/// </summary>
	internal abstract class InstanceValidatorManagerBase : ValidatorManagerBase
	{
		protected readonly List<Object> _objectsToValidate;
		protected readonly Dictionary<Type, List<AbstractInstanceValidator>> _typeToValidatorsLookup;
		protected readonly ClassTypeCache _cache;
		protected readonly InstanceValidatorCache _instanceValidatorCache;

		protected InstanceValidatorManagerBase(ClassTypeCache cache, LogCache logCache)
			: base(logCache)
		{
			_cache = cache;
			_instanceValidatorCache = new InstanceValidatorCache();
			for (var i = 0; i < _instanceValidatorCache.Count; i++)
			{
				_instanceValidatorCache[i].LogCreated += OnLogCreated;
			}

			// Build up a lookup table of type to validators to be able to apply that set of validators to
			// an instance of that type later on.
			_objectsToValidate = new List<Object>();
			_typeToValidatorsLookup = new Dictionary<Type, List<AbstractInstanceValidator>>();

			for (var i = 0; i < _cache.Count; i++)
			{
				var cacheType = _cache[i];
				for (var j = 0; j < _instanceValidatorCache.Count; j++)
				{
					var validator = _instanceValidatorCache[j];
					if (!validator.AppliesTo(cacheType))
					{
						continue;
					}

					if (!_typeToValidatorsLookup.ContainsKey(cacheType))
					{
						_typeToValidatorsLookup.Add(cacheType, new List<AbstractInstanceValidator>());
					}

					_typeToValidatorsLookup[cacheType].Add(validator);
				}
			}
		}

		#pragma warning disable UEA0008 // Unsealed Derived Class
		public override void ValidateAll()
		{
			for (var i = 0; i < _objectsToValidate.Count; i++)
			{
				var evalType = _objectsToValidate[i].GetType();
				if (!_typeToValidatorsLookup.ContainsKey(evalType))
				{
					continue;
				}

				var validators = _typeToValidatorsLookup[evalType];
				for (var j = 0; j < validators.Count; j++)
				{
					validators[j].Validate(_objectsToValidate[i]);
				}
			}

			// Mark continuous progress as the total count to ensure the ActiveSceneValidator shows as complete.
			_continuousProgress = _objectsToValidate.Count;
		}

		public override bool ContinueValidation()
		{
			if (_continuousProgress >= _objectsToValidate.Count)
			{
				return false;
			}

			var nextStep = _continuousProgress + _continuousObjectsPerStep >= _objectsToValidate.Count
				? _objectsToValidate.Count
				: _continuousProgress + _continuousObjectsPerStep;

			for (; _continuousProgress < nextStep; _continuousProgress++)
			{
				var evalType = _objectsToValidate[_continuousProgress].GetType();
				if (!_typeToValidatorsLookup.ContainsKey(evalType))
				{
					continue;
				}

				var validators = _typeToValidatorsLookup[evalType];
				for (var j = 0; j < validators.Count; j++)
				{
					validators[j].Validate(_objectsToValidate[_continuousProgress]);
				}
			}

			return _continuousProgress < _objectsToValidate.Count;
		}

		public override bool IsComplete()
		{
			return _continuousProgress >= _objectsToValidate.Count;
		}

		public override float GetProgress()
		{
			return _continuousProgress / (float)_objectsToValidate.Count;
		}
		#pragma warning restore UEA0008 // Unsealed Derived Class

		/// <summary>
		/// Only used for unit tests to be able to verify that the objects discovered are the ones expected
		/// by a test.
		/// </summary>
		/// <returns></returns>
		internal List<Object> GetObjectsToValidate()
		{
			return _objectsToValidate;
		}
	}
}

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
using UnityEditor;
using UnityEngine;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="ProjectAssetValidatorManager"/> is a validator manager that inspects all relevant
	/// <see cref="Object"/> instances (prefabs) in the Assets folder as well as executing validation using
	/// all <see cref="ProjectValidatorBase"/> derived classes with a <see cref="ValidatorAttribute"/>.
	/// </summary>
	internal sealed class ProjectAssetValidatorManager : InstanceValidatorManagerBase
	{
		private readonly string[] _allPrefabGUIDs;
		private int _continueSearchProgress;
		private int _projectSearchProgress;
		private int _projectValidationProgress;
		private bool _hasSearchedProjectValidatorCache;
		private bool _hasValidatedUsingProjectValidatorCache;
		private readonly ProjectValidatorCache _projectValidatorCache;

		private const string PrefabWildcardFilter = "t:Prefab";

		public ProjectAssetValidatorManager(ClassTypeCache cache, LogCache logCache)
			: base(cache, logCache)
		{
			_continueSearchProgress = 0;
			_allPrefabGUIDs = AssetDatabase.FindAssets(PrefabWildcardFilter);
			_projectValidatorCache = new ProjectValidatorCache();
			for (var i = 0; i < _projectValidatorCache.Count; i++)
			{
				_projectValidatorCache[i].LogCreated += OnLogCreated;
			}
		}

		public override void Search()
		{
			_objectsToValidate.Clear();

			// Get all prefab locations for instance validators
			for (var i = 0; i < _allPrefabGUIDs.Length; i++)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(_allPrefabGUIDs[i]);
				var prefabObject = AssetDatabase.LoadMainAssetAtPath(assetPath);
				var prefabGameObject = prefabObject as GameObject;
				if (prefabGameObject == null)
				{
					continue;
				}

				for (var j = 0; j < _cache.Count; j++)
				{
					var components = prefabGameObject.GetComponentsInChildren(_cache[j]);
					if (components.Length == 0)
					{
						continue;
					}

					for (var h = 0; h < components.Length; h++)
					{
						_objectsToValidate.Add(components[h]);
					}
				}
			}

			// Map all project asset validators and Search
			for (var i = 0; i < _projectValidatorCache.Count; i++)
			{
				_projectValidatorCache[i].Search();
			}

			_hasSearchedProjectValidatorCache = true;
		}

		public bool ContinueSearch()
		{
			var nextStep = _continueSearchProgress + _continuousObjectsPerStep >= _allPrefabGUIDs.Length
				? _allPrefabGUIDs.Length
				: _continueSearchProgress + _continuousObjectsPerStep;

			for (; _continueSearchProgress < nextStep; _continueSearchProgress++)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(_allPrefabGUIDs[_continueSearchProgress]);
				var prefabObject = AssetDatabase.LoadMainAssetAtPath(assetPath);
				var prefabGameObject = prefabObject as GameObject;
				if (prefabGameObject == null)
				{
					continue;
				}

				for (var j = 0; j < _cache.Count; j++)
				{
					var components = prefabGameObject.GetComponentsInChildren(_cache[j]);
					if (components.Length == 0)
					{
						continue;
					}

					for (var h = 0; h < components.Length; h++)
					{
						_objectsToValidate.Add(components[h]);
					}
				}
			}

			// Iterate one at a time through all project asset validators and validate
			nextStep = _projectSearchProgress + 1 >= _projectValidatorCache.Count
				? _projectValidatorCache.Count
				: _projectSearchProgress + 1;

			for (; _projectSearchProgress < nextStep; _projectSearchProgress++)
			{
				_projectValidatorCache[_projectSearchProgress].Search();
			}

			_hasSearchedProjectValidatorCache = _projectSearchProgress >= _projectValidatorCache.Count;

			// Return false once we've run searching through all prefabs and project validators
			if (_continueSearchProgress < _allPrefabGUIDs.Length)
			{
				return false;
			}

			if (!_hasSearchedProjectValidatorCache)
			{
				return false;
			}

			return true;
		}

		public float GetSearchProgress()
		{
			return Mathf.Clamp01((_continueSearchProgress + _projectSearchProgress) /
			                     ((float)_allPrefabGUIDs.Length + _projectValidatorCache.Count));
		}

		public bool IsSearchComplete()
		{
			return GetSearchProgress() >= 1f && _hasSearchedProjectValidatorCache;
		}

		public override void ValidateAll()
		{
			base.ValidateAll();

			// Map all project asset validators and Search
			for (var i = 0; i < _projectValidatorCache.Count; i++)
			{
				_projectValidatorCache[i].Validate();
			}

			_hasValidatedUsingProjectValidatorCache = true;
		}

		public override bool ContinueValidation()
		{
			// Iterate one at a time through all project asset validators and validate
			var nextStep = _projectValidationProgress + 1 >= _projectValidatorCache.Count
				? _projectValidatorCache.Count
				: _projectValidationProgress + 1;

			for (; _projectValidationProgress < nextStep; _projectValidationProgress++)
			{
				_projectValidatorCache[_projectValidationProgress].Validate();
			}

			_hasValidatedUsingProjectValidatorCache = _projectValidationProgress >= _projectValidatorCache.Count;

			// Return true once we've run through all prefabs and project validators
			return base.ContinueValidation() || _projectValidationProgress < _projectValidatorCache.Count;
		}

		public override float GetProgress()
		{
			return (_continuousProgress + _projectValidationProgress) /
			       ((float)_objectsToValidate.Count + _projectValidatorCache.Count);
		}

		public override bool IsComplete()
		{
			return base.IsComplete() && _hasValidatedUsingProjectValidatorCache;
		}

		protected override void OnLogCreated(ValidationLog validationLog)
		{
			validationLog.source = LogSource.Project;

			base.OnLogCreated(validationLog);
		}
	}
}

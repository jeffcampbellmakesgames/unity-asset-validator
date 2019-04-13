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
using UnityEngine;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="CrossSceneValidatorManager"/> is a validator manager that validates one or more scenes
	/// using all derived validators of <see cref="CrossSceneValidatorBase"/> with a decorated
	/// <see cref="ValidatorAttribute"/>.
	/// </summary>
	internal sealed class CrossSceneValidatorManager : ValidatorManagerBase
	{
		private readonly CrossSceneValidatorCache _crossSceneValidatorCache;

		public CrossSceneValidatorManager(LogCache logCache)
			: base(logCache)
		{
			_crossSceneValidatorCache = new CrossSceneValidatorCache();

			for (var i = 0; i < _crossSceneValidatorCache.Count; i++)
			{
				_crossSceneValidatorCache[i].LogCreated += logCache.OnLogCreated;
			}
		}

		public override bool IsComplete()
		{
			return _continuousProgress >= _crossSceneValidatorCache.Count;
		}

		public override float GetProgress()
		{
			return Mathf.Clamp01(_continuousProgress / (float)_crossSceneValidatorCache.Count);
		}

		public override void Search()
		{
			for (var i = 0; i < _crossSceneValidatorCache.Count; i++)
			{
				_crossSceneValidatorCache[i].Search();
			}
		}

		public override void ValidateAll()
		{
			for (; _continuousProgress < _crossSceneValidatorCache.Count; _continuousProgress++)
			{
				_crossSceneValidatorCache[_continuousProgress].Validate();
			}
		}

		public override bool ContinueValidation()
		{
			if (_continuousProgress >= _crossSceneValidatorCache.Count)
			{
				return false;
			}

			var nextStep = _continuousProgress + _continuousObjectsPerStep >= _crossSceneValidatorCache.Count
				? _crossSceneValidatorCache.Count
				: _continuousProgress + _continuousObjectsPerStep;

			for (; _continuousProgress < nextStep; _continuousProgress++)
			{
				_crossSceneValidatorCache[_continuousProgress].Validate();
			}

			return _continuousProgress < _crossSceneValidatorCache.Count;
		}
	}
}

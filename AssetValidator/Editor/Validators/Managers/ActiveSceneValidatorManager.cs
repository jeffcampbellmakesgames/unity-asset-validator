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
using UnityEngine.SceneManagement;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="ActiveSceneValidatorManager"/> inspects components in a single scene whose type
	/// is contained in the passed <see cref="ClassTypeCache"/> instance and outputs any logs for validation to
	/// the passed <see cref="LogCache"/> instance.
	/// </summary>
	internal sealed class ActiveSceneValidatorManager : InstanceValidatorManagerBase
	{
		public ActiveSceneValidatorManager(ClassTypeCache cache, LogCache logCache)
			: base(cache, logCache)
		{
		}

		public override void Search()
		{
			_objectsToValidate.Clear();
			for (var i = 0; i < _cache.Count; i++)
			{
				_objectsToValidate.AddRange(Object.FindObjectsOfType(_cache[i]));
			}
		}

		protected override void OnLogCreated(ValidationLog validationLog)
		{
			validationLog.scenePath = SceneManager.GetActiveScene().path;
			validationLog.source = LogSource.Scene;

			base.OnLogCreated(validationLog);
		}
	}
}

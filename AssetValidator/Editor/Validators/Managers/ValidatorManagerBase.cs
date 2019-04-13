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

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="ValidatorManagerBase"/> is used as the base class for any validator manager that is
	/// responsible for an area of validation.
	/// </summary>
	internal abstract class ValidatorManagerBase
	{
		/// <summary>
		/// The cache of validation logs that validators should be adding to when validating.
		/// </summary>
		protected readonly LogCache _logCache;

		protected int _continuousProgress;
		protected const int _continuousObjectsPerStep = 1;

		protected ValidatorManagerBase(LogCache logCache)
		{
			_logCache = logCache;
		}

		/// <summary>
		/// Search for any validation targets.
		/// </summary>
		public virtual void Search()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Synchronously validate all validation targets at once.
		/// </summary>
		public virtual void ValidateAll()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Synchronously validate a subset of validation targets at once and return true if all targets
		/// have been validated, otherwise false.
		/// </summary>
		public virtual bool ContinueValidation()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Return true if validation has been completed, otherwise false.
		/// </summary>
		/// <returns></returns>
		public virtual bool IsComplete()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Return a normalized scalar value [0-1] indicating the overall progress of validation.
		/// </summary>
		/// <returns></returns>
		public virtual float GetProgress()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Invoked when a validator has emitted a <see cref="ValidationLog"/> via an event, adds
		/// it to the cache if there is one.
		/// </summary>
		/// <param name="validationLog"></param>
		protected virtual void OnLogCreated(ValidationLog validationLog)
		{
			_logCache.OnLogCreated(validationLog);
		}
	}
}

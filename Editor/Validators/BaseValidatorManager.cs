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
using JCMG.AssetValidator.Editor.Validators.Output;
using System;

namespace JCMG.AssetValidator.Editor.Validators
{
    /// <summary>
    /// The BaseValidatorManager class is used as the base class for any manager that is
    /// validating using types held in a ClassTypeCache by using VFieldValidators.
    /// </summary>
    public class BaseValidatorManager : IDisposable
    {
        protected readonly AssetValidatorLogger _logger;

        protected int _continousProgress;
        protected const int _continuousObjectsPerStep = 1;

        protected BaseValidatorManager(AssetValidatorLogger logger)
        {
            _logger = logger;
        }

        public virtual void Search()
        {
            throw new NotImplementedException();
        }

        public virtual void ValidateAll()
        {
            throw new NotImplementedException();
        }

        public virtual bool ContinueValidation()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsComplete()
        {
            throw new NotImplementedException();
        }

        public virtual float GetProgress()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnLogEvent(VLog vLog)
        {
            _logger.OnLogEvent(vLog);
        }

        #region IDisposable

        public virtual void Dispose() { }

        #endregion
    }
}
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
using JCMG.AssetValidator.Editor.Meta;
using JCMG.AssetValidator.Editor.Validators.Output;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Validators
{
    public class CrossSceneValidatorManager : BaseValidatorManager
    {
        private readonly CrossSceneValidatorCache _crossSceneValidatorCache;

        public CrossSceneValidatorManager(AssetValidatorLogger logger) 
            : base(logger)
        {
            _crossSceneValidatorCache = new CrossSceneValidatorCache();

            for (var i = 0; i < _crossSceneValidatorCache.Count; i++)
                _crossSceneValidatorCache[i].OnLogEvent += _logger.OnLogEvent;
        }

        public override bool IsComplete()
        {
            return _continousProgress >= _crossSceneValidatorCache.Count;
        }

        public override float GetProgress()
        {
            return Mathf.Clamp01(_continousProgress / (float)_crossSceneValidatorCache.Count);
        }

        public override void Search()
        {
            for (var i = 0; i < _crossSceneValidatorCache.Count; i++)
                _crossSceneValidatorCache[i].Search();
        }

        public override void ValidateAll()
        {
            for (; _continousProgress < _crossSceneValidatorCache.Count; _continousProgress++)
                _crossSceneValidatorCache[_continousProgress].Validate();
        }

        public override bool ContinueValidation()
        {
            if (_continousProgress >= _crossSceneValidatorCache.Count) return false;

            var nextStep = _continousProgress + _continuousObjectsPerStep >= _crossSceneValidatorCache.Count
                ? _crossSceneValidatorCache.Count
                : _continousProgress + _continuousObjectsPerStep;

            for (; _continousProgress < nextStep; _continousProgress++)
                _crossSceneValidatorCache[_continousProgress].Validate();

            return _continousProgress < _crossSceneValidatorCache.Count;
        }

        #region IDisposable

        public sealed override void Dispose()
        {
            for (var i = 0; i < _crossSceneValidatorCache.Count; i++)
                _crossSceneValidatorCache[i].OnLogEvent -= _logger.OnLogEvent;

            base.Dispose();
        }

        #endregion
    }
}

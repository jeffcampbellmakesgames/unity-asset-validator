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
#if UNITY_5_6_OR_NEWER
using System.Collections;
using JCMG.AssetValidator.Editor.Meta;
using JCMG.AssetValidator.Editor.Validators.FieldValidators;
using JCMG.AssetValidator.Editor.Validators.Output;
using JCMG.AssetValidator.UnitTestObjects;
using NUnit.Framework;
using UnityEngine;

using UnityEngine.TestTools;

namespace JCMG.AssetValidator.Editor.Validators.Tests
{
    [TestFixture]
    public class SceneValidatorTests
    {
        private ActiveSceneValidatorManager _sValidatorManager;
        private ClassTypeCache _coreCache;
        private AssetValidatorLogger _logger;

        [SetUp]
        public void Setup()
        {
            _coreCache = new ClassTypeCache();
            _coreCache.AddTypeWithAttribute<Monobehavior2, ValidateAttribute>();
            _logger = new AssetValidatorLogger();

            _sValidatorManager = new ActiveSceneValidatorManager(_coreCache, _logger);
        }

        [TearDown]
        public void Teardown()
        {
            if (_sValidatorManager != null) _sValidatorManager.Dispose();
        }

        [UnityTest]
        public IEnumerator AssertThatSceneSearcherCanFindObjectsToValidate()
        {
            _sValidatorManager.Search();

            Assert.AreEqual(0, _sValidatorManager.GetObjectsToValidate().Count);

            yield return null;

            var validObjectOne = new GameObject();
            var validObjectTwo = new GameObject();
            var validObjectThree = new GameObject();

            var sceneParserTestObjectA = validObjectOne.AddComponent<SceneParserTestObjectA>();
            validObjectTwo.AddComponent<SceneParserTestObjectB>();
            validObjectThree.AddComponent<SceneParserTestObjectB>();

            yield return null;

            _sValidatorManager.Search();

            var objectsToValidate = _sValidatorManager.GetObjectsToValidate();
            Assert.Contains(sceneParserTestObjectA, objectsToValidate, 
                "ObjectsToValidate should contain a SceneParserTestObjectA...");
            Assert.AreEqual(3, objectsToValidate.Count);

            Object.DestroyImmediate(validObjectOne);
            Object.DestroyImmediate(validObjectTwo);
            Object.DestroyImmediate(validObjectThree);
        }
        
        [UnityTest]
        public IEnumerator AssertThatSceneSearcherCanValidateObjects()
        {
            var validObjectOne = new GameObject("TargetToEval");
            var validObjectTwo = new GameObject("TargetToIgnore");
            var validObjectThree = new GameObject("TargetTo");

            validObjectOne.AddComponent<SceneParserTestObjectA>();

            yield return null;

            _sValidatorManager.Search();
            _sValidatorManager.ValidateAll();

            validObjectTwo.AddComponent<SceneParserTestObjectB>();
            validObjectThree.AddComponent<SceneParserTestObjectC>();

            yield return null;

            _sValidatorManager.Search();
            _sValidatorManager.ValidateAll();

            Object.DestroyImmediate(validObjectOne);
            Object.DestroyImmediate(validObjectTwo);
            Object.DestroyImmediate(validObjectThree);
        }

        public class SceneParserTestAttribute : FieldAttribute { }

        [OnlyIncludeInTests]
        [FieldTarget("scene_parser_test_validator", typeof(SceneParserTestAttribute))]
        public class SceneParserTestValidator : BaseFieldValidator
        {
            public override bool Validate(Object obj)
            {
                return obj.GetType() != typeof(SceneParserTestObjectA);
            }
        }
        
        [Validate]
        public class SceneParserTestObjectA : Monobehavior2
        {
            [SceneParserTest]
            public object obj;
        }
        
        [Validate]
        public class SceneParserTestObjectB : Monobehavior2
        {
            public object obj;
        }
        
        [Validate]
        public class SceneParserTestObjectC : Monobehavior2
        {
            [SceneParserTest]
            public object obj;
        }
    }
}
#endif
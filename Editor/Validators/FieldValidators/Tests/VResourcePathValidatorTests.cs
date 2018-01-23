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
using NUnit.Framework;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JCMG.AssetValidator.Editor.Validators.FieldValidators.Tests
{
    [TestFixture]
    public class VResourcePathValidatorTests
    {
        private ResourcePathValidator _vrpValidator;
        private VResourcePathTestObjectA _vResourcePathTestObjectA;
        private VResourcePathTestObjectB _vResourcePathTestObjectB;
        private GameObject _gameObject;

        [SetUp]
        public void Setup()
        {
            _vrpValidator = new ResourcePathValidator();
            _gameObject = new GameObject();
            _vResourcePathTestObjectA = _gameObject.AddComponent<VResourcePathTestObjectA>();
            _vResourcePathTestObjectB = ScriptableObject.CreateInstance<VResourcePathTestObjectB>();
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(_gameObject);
            Object.DestroyImmediate(_vResourcePathTestObjectB);
        }

        private void ToggleResources(bool isValidResources)
        {
            var resourcePath = isValidResources ? "ProjectPrefabSearchTest" : "Invalid path";

            _vResourcePathTestObjectA.resourcePath = resourcePath;
            _vResourcePathTestObjectA.helper = new ResourcePathHelper(resourcePath);

            _vResourcePathTestObjectB.resourcePath = resourcePath;
            _vResourcePathTestObjectB.helper = new ResourcePathHelper(resourcePath);
        }

        [Test]
        [Ignore("This test is skipped due to not wanting to include unessary Resources folder in projects using this library. " +
                "See the readme.md file in the resource_unit_test folder for details on running these tests")]
        public void AssertThatValidatorValidatesTrueOnResourcePath()
        {
            ToggleResources(true);
            
            // Monobehavior
            var aFields = _vrpValidator.GetFieldInfosApplyTo(_vResourcePathTestObjectA);

            Assert.AreEqual(2, aFields.Count());

            Assert.True(_vrpValidator.AppliesTo(_vResourcePathTestObjectA));
            Assert.True(_vrpValidator.Validate(_vResourcePathTestObjectA));

            // ScriptableObject
            var bFields = _vrpValidator.GetFieldInfosApplyTo(_vResourcePathTestObjectB);

            Assert.AreEqual(2, bFields.Count());

            Assert.True(_vrpValidator.AppliesTo(_vResourcePathTestObjectB));
            Assert.True(_vrpValidator.Validate(_vResourcePathTestObjectB));
        }

        [Test]
        [Ignore("This test is skipped due to not wanting to include unessary Resources folder in projects using this library. " +
                "See the readme.md file in the resource_unit_test folder for details on running these tests")]
        public void AssertThatValidatorValidatesFalseOnResourcePathString()
        {
            ToggleResources(false);

            // Monobehavior
            var aFields = _vrpValidator.GetFieldInfosApplyTo(_vResourcePathTestObjectA);

            Assert.AreEqual(2, aFields.Count());

            Assert.True(_vrpValidator.AppliesTo(_vResourcePathTestObjectA));
            Assert.False(_vrpValidator.Validate(_vResourcePathTestObjectA));

            // ScriptableObject
            var bFields = _vrpValidator.GetFieldInfosApplyTo(_vResourcePathTestObjectB);

            Assert.AreEqual(2, bFields.Count());

            Assert.True(_vrpValidator.AppliesTo(_vResourcePathTestObjectB));
            Assert.False(_vrpValidator.Validate(_vResourcePathTestObjectB));
        }

        public class VResourcePathTestObjectA : MonoBehaviour
        {
            [ResourcePath]
            public string resourcePath;

            [ResourcePath]
            public ResourcePathHelper helper;
        }

        public class VResourcePathTestObjectB : ScriptableObject
        {
            [ResourcePath]
            public string resourcePath;

            [ResourcePath]
            public ResourcePathHelper helper;
        }

        public class ResourcePathHelper
        {
            private readonly string _path;

            public ResourcePathHelper(string path)
            {
                _path = path;
            }

            public override string ToString()
            {
                return _path;
            }
        }
    }
}

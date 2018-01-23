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
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Validators.FieldValidators.Tests
{
    [TestFixture]
    public class VNonNullValidatorTests
    {
        private NonNullValidator _validator;
        private VNonNullFieldTestObjectA _objectA;
        private VNonNullFieldTestObjectB _objectB;
        private GameObject _gameObject;

        [SetUp]
        public void Setup()
        {
            _validator = new NonNullValidator();

            _gameObject = new GameObject();
            _objectA = _gameObject.AddComponent<VNonNullFieldTestObjectA>();
            _objectB = _gameObject.AddComponent<VNonNullFieldTestObjectB>();
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void AssertThatFieldValidatorValidatesFalseOnNonNullRef()
        {
            Assert.False(_validator.Validate(_objectA));
        }
        
        [Test]
        public void AssertThatFieldValidatorValidatesTrueOnNonNullRef()
        {
            Assert.True(_validator.Validate(_objectB));
        }
    }

    [OnlyIncludeInTests]
    [Validate]
    public class VNonNullFieldTestObjectA : MonoBehaviour
    {
        [NonNull]
        public object obj;
    }

    [OnlyIncludeInTests]
    [Validate]
    public class VNonNullFieldTestObjectB : MonoBehaviour
    {
        public object obj;
    }
}

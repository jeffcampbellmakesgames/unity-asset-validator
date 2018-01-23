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
using JCMG.AssetValidator.UnitTestObjects;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Validators.FieldValidators.Tests
{
    [TestFixture]
    public class VFieldValidatorTests
    {
        private FieldTestValidator _validator;
        private VFieldTestObjectA _objectA;
        private VFieldTestObjectB _objectB;

        private GameObject _gameObject;

        [SetUp]
        public void Setup()
        {
            _gameObject = new GameObject();

            _validator = new FieldTestValidator();
            _objectA = _gameObject.AddComponent<VFieldTestObjectA>();
            _objectB = _gameObject.AddComponent<VFieldTestObjectB>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void AssertThatFieldValidatorAppliesToInstance()
        {
            var fields = _validator.GetFieldInfosApplyTo(_objectA);

            Assert.AreEqual(1, fields.Count());
            Assert.True(_validator.AppliesTo(_objectA));
        }

        [Test]
        public void AssertThatFieldValidatorDoesNotApplyToInstance()
        {
            var fields = _validator.GetFieldInfosApplyTo(_objectB);

            Assert.AreEqual(0, fields.Count());
            Assert.False(_validator.AppliesTo(_objectB));
        }
    }

    public class FieldTestAttribute : FieldAttribute { }

    [OnlyIncludeInTests]
    [FieldTarget("field_test_validator", typeof(FieldTestAttribute))]
    public class FieldTestValidator : BaseFieldValidator
    {
        public override bool Validate(Object obj)
        {
            return false;
        }
    }
    
    [Validate]
    public class VFieldTestObjectA : Monobehavior2
    {
        [FieldTest]
        public object obj;
    }
    
    [Validate]
    public class VFieldTestObjectB : Monobehavior2
    {
        public object obj;
    }
}

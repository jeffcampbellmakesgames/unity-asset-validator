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
using Object = UnityEngine.Object;

namespace JCMG.AssetValidator.Editor.Validators.ObjectValidators.Tests
{
    [TestFixture]
    public class VObjectValidatorTests
    {
        private MonobehaviorObjectTestValidator _mbValidator;
        private ScriptableObjectTestValidator _soValidator;

        // Monobehaviors
        private VObjectTestA _vObjectTestA;
        private VObjectTestC _vObjectTestC;

        // ScriptableObjects
        private VObjectTestB _vObjectTestB;
        private VObjectTestD _vObjectTestD;

        private GameObject _gameObject;

        [SetUp]
        public void Setup()
        {
            _gameObject = new GameObject();
            _vObjectTestA = _gameObject.AddComponent<VObjectTestA>();
            _vObjectTestC = _gameObject.AddComponent<VObjectTestC>();

            _vObjectTestB = ScriptableObject.CreateInstance<VObjectTestB>();
            _vObjectTestD = ScriptableObject.CreateInstance<VObjectTestD>();

            _mbValidator = new MonobehaviorObjectTestValidator();
            _soValidator = new ScriptableObjectTestValidator();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
            Object.DestroyImmediate(_vObjectTestB);
            Object.DestroyImmediate(_vObjectTestD);
        }

        [Test]
        public void AssertThatObjectValidatorCanApplyOnlyInstanceOfMonobehavior()
        {
            Assert.True(_mbValidator.AppliesTo(_vObjectTestA)); // Can apply to decorated Monobehavior derived class
            Assert.False(_mbValidator.AppliesTo(_vObjectTestB)); // Cannot apply to decorated ScriptableObject
            Assert.False(_mbValidator.AppliesTo(typeof(VObjectTestE))); // Cannot apply to decorated POCO class
        }


        [Test]
        public void AssertThatObjectValidatorCanApplyOnlyToInstanceOfScriptableObject()
        {
            Assert.True(_soValidator.AppliesTo(_vObjectTestD)); // Can apply to decorated ScriptableObject
            Assert.False(_soValidator.AppliesTo(_vObjectTestC)); // Cannot apply to decorated Monobehavior
            Assert.False(_soValidator.AppliesTo(typeof(VObjectTestF))); // Cannot apply to decorated POCO class
        }

        public class ObjectTestMonobehaviorAttribute : ValidateAttribute { }

        public class ObjectTestScriptableObjectAttribute : ValidateAttribute
        {
            public ObjectTestScriptableObjectAttribute() : base(UnityTarget.ScriptableObject) { }
        }

        [OnlyIncludeInTests]
        [ObjectTarget("monobehavior_object_test_validator", typeof(ObjectTestMonobehaviorAttribute))]
        public class MonobehaviorObjectTestValidator : BaseObjectValidator
        {
            public override bool Validate(Object o)
            {
                return false;
            }
        }

        [OnlyIncludeInTests]
        [ObjectTarget("scriptable_object_test_validator", typeof(ObjectTestScriptableObjectAttribute))]
        public class ScriptableObjectTestValidator : BaseObjectValidator
        {
            public override bool Validate(Object o)
            {
                return false;
            }
        }

        // Test objects for monobehavior only attribute
        /// <summary>
        /// Valid for this attribute
        /// </summary>
        [ObjectTestMonobehavior]
        public class VObjectTestA : MonoBehaviour
        {
            public object obj;
        }

        /// <summary>
        /// Invalid for this attribute
        /// </summary>
        [ObjectTestMonobehavior]
        public class VObjectTestB : ScriptableObject
        {
            public object obj;
        }

        // Test objects for scriptable object only attribute
        /// <summary>
        /// Invalid for this attribute
        /// </summary>
        [ObjectTestScriptableObject]
        public class VObjectTestC : MonoBehaviour
        {
            public object obj;
        }

        /// <summary>
        /// Valid for this attribute
        /// </summary>
        [ObjectTestScriptableObject]
        public class VObjectTestD : ScriptableObject
        {
            public object obj;
        }

        // Test POCO objects for both attributes
        /// <summary>
        /// Invalid for this attribute
        /// </summary>
        [ObjectTestMonobehavior]
        public class VObjectTestE
        {
            public object obj;
        }

        /// <summary>
        /// Valid for this attribute
        /// </summary>
        [ObjectTestScriptableObject]
        public class VObjectTestF
        {
            public object obj;
        }
    }
}

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
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Tests
{
	[TestFixture]
	public class FieldValidatorTests
	{
		private FieldTestValidator _validator;
		private FieldTestObjectA _objectA;
		private FieldTestObjectB _objectB;

		private GameObject _gameObject;

		[SetUp]
		public void Setup()
		{
			_gameObject = new GameObject();

			_validator = new FieldTestValidator();
			_objectA = _gameObject.AddComponent<FieldTestObjectA>();
			_objectB = _gameObject.AddComponent<FieldTestObjectB>();
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

		#pragma warning disable 0649

		private class FieldTestAttribute : AssetValidator.FieldAttribute
		{
		}

		[OnlyIncludeInTests]
		[FieldValidator("field_test_validator", typeof(FieldTestAttribute))]
		private class FieldTestValidator : FieldValidatorBase
		{
			public override bool Validate(Object obj)
			{
				return false;
			}
		}

		[Validate]
		private class FieldTestObjectA : MonoBehaviourTwo
		{
			[FieldTest]
			public object obj;
		}

		[Validate]
		private class FieldTestObjectB : MonoBehaviourTwo
		{
			public object obj;
		}

		#pragma warning restore 0649
	}
}

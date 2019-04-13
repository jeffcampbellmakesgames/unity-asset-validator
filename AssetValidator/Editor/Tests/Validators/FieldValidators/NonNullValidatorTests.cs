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
using NUnit.Framework;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Tests
{
	[TestFixture]
	internal class NonNullValidatorTests
	{
		private NonNullFieldValidator _fieldValidator;
		private NonNullFieldTestObjectA _objectA;
		private NonNullFieldTestObjectB _objectB;
		private GameObject _gameObject;

		[SetUp]
		public void Setup()
		{
			_fieldValidator = new NonNullFieldValidator();

			_gameObject = new GameObject();
			_objectA = _gameObject.AddComponent<NonNullFieldTestObjectA>();
			_objectB = _gameObject.AddComponent<NonNullFieldTestObjectB>();
		}

		[TearDown]
		public void Cleanup()
		{
			Object.DestroyImmediate(_gameObject);
		}

		[Test]
		public void AssertThatFieldValidatorValidatesFalseOnNonNullRef()
		{
			Assert.False(_fieldValidator.Validate(_objectA));
		}

		[Test]
		public void AssertThatFieldValidatorValidatesTrueOnNonNullRef()
		{
			Assert.True(_fieldValidator.Validate(_objectB));
		}

		#pragma warning disable 0649

		[OnlyIncludeInTests]
		[Validate]
		private class NonNullFieldTestObjectA : MonoBehaviour
		{
			[NonNull]
			public object obj;
		}

		[OnlyIncludeInTests]
		[Validate]
		private class NonNullFieldTestObjectB : MonoBehaviour
		{
			public object obj;
		}

		#pragma warning restore 0649
	}
}

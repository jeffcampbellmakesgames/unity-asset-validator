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
	internal class IsProjectReferenceValidatorTests
	{
		private IsProjectReferenceFieldValidator _pFieldValidator;
		private GameObject _gameObject;
		private ProjectRefObjectA _projectRefObjectA;
		private const string UNIT_TEST_RESOURCE_PATH = "ProjectPrefabSearchTest";

		[SetUp]
		public void Setup()
		{
			_pFieldValidator = new IsProjectReferenceFieldValidator();
			_gameObject = new GameObject();
			_projectRefObjectA = _gameObject.AddComponent<ProjectRefObjectA>();
		}

		[TearDown]
		public void TearDown()
		{
			Object.DestroyImmediate(_gameObject);
		}

		[Test]
		[Ignore(
			"This test is skipped due to not wanting to include unnecessary Resources folder in projects using this library. " +
			"See the readme.md file in the resource_unit_test folder for details on running these tests")]
		public void AssertThatValidatorReturnsFalseForNullReference()
		{
			Assert.False(_pFieldValidator.Validate(_projectRefObjectA));
		}

		[Test]
		[Ignore(
			"This test is skipped due to not wanting to include unnecessary Resources folder in projects using this library. " +
			"See the readme.md file in the resource_unit_test folder for details on running these tests")]
		public void AssertThatValidatorReturnsFalseForSceneReference()
		{
			var gameObjectSceneRef = new GameObject();
			_projectRefObjectA.projectRefField = gameObjectSceneRef;

			Assert.False(_pFieldValidator.Validate(_projectRefObjectA));
		}

		[Test]
		[Ignore(
			"This test is skipped due to not wanting to include unnecessary Resources folder in projects using this library. " +
			"See the readme.md file in the resource_unit_test folder for details on running these tests")]
		public void AssertThatValidatorReturnsTrueForProjectReference()
		{
			var projectRef = Resources.Load(UNIT_TEST_RESOURCE_PATH);
			_projectRefObjectA.projectRefField = projectRef;

			Assert.True(_pFieldValidator.Validate(_projectRefObjectA));
		}

		private class ProjectRefObjectA : MonoBehaviour
		{
			[IsProjectReference]
			public Object projectRefField;
		}
	}
}

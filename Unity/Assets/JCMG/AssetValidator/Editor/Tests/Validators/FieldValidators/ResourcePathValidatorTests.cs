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
	internal class ResourcePathValidatorTests
	{
		private ResourcePathFieldValidator _vrpFieldValidator;
		private ResourcePathTestObjectA _resourcePathTestObjectA;
		private ResourcePathTestObjectB _resourcePathTestObjectB;
		private GameObject _gameObject;

		[SetUp]
		public void Setup()
		{
			_vrpFieldValidator = new ResourcePathFieldValidator();
			_gameObject = new GameObject();
			_resourcePathTestObjectA = _gameObject.AddComponent<ResourcePathTestObjectA>();
			_resourcePathTestObjectB = ScriptableObject.CreateInstance<ResourcePathTestObjectB>();
		}

		[TearDown]
		public void Cleanup()
		{
			Object.DestroyImmediate(_gameObject);
			Object.DestroyImmediate(_resourcePathTestObjectB);
		}

		private void ToggleResources(bool isValidResources)
		{
			var resourcePath = isValidResources ? "ProjectPrefabSearchTest" : "Invalid path";

			_resourcePathTestObjectA.resourcePath = resourcePath;
			_resourcePathTestObjectA.helper = new ResourcePathHelper(resourcePath);

			_resourcePathTestObjectB.resourcePath = resourcePath;
			_resourcePathTestObjectB.helper = new ResourcePathHelper(resourcePath);
		}

		[Test]
		[Ignore(
			"This test is skipped due to not wanting to include unnecessary Resources folder in projects using this library. " +
			"See the readme.md file in the resource_unit_test folder for details on running these tests")]
		public void AssertThatValidatorValidatesTrueOnResourcePath()
		{
			ToggleResources(true);

			// MonoBehavior
			var aFields = _vrpFieldValidator.GetFieldInfosApplyTo(_resourcePathTestObjectA);

			Assert.AreEqual(2, aFields.Count());

			Assert.True(_vrpFieldValidator.AppliesTo(_resourcePathTestObjectA));
			Assert.True(_vrpFieldValidator.Validate(_resourcePathTestObjectA));

			// ScriptableObject
			var bFields = _vrpFieldValidator.GetFieldInfosApplyTo(_resourcePathTestObjectB);

			Assert.AreEqual(2, bFields.Count());

			Assert.True(_vrpFieldValidator.AppliesTo(_resourcePathTestObjectB));
			Assert.True(_vrpFieldValidator.Validate(_resourcePathTestObjectB));
		}

		[Test]
		[Ignore(
			"This test is skipped due to not wanting to include unnecessary Resources folder in projects using this library. " +
			"See the readme.md file in the resource_unit_test folder for details on running these tests")]
		public void AssertThatValidatorValidatesFalseOnResourcePathString()
		{
			ToggleResources(false);

			// MonoBehavior
			var aFields = _vrpFieldValidator.GetFieldInfosApplyTo(_resourcePathTestObjectA);

			Assert.AreEqual(2, aFields.Count());

			Assert.True(_vrpFieldValidator.AppliesTo(_resourcePathTestObjectA));
			Assert.False(_vrpFieldValidator.Validate(_resourcePathTestObjectA));

			// ScriptableObject
			var bFields = _vrpFieldValidator.GetFieldInfosApplyTo(_resourcePathTestObjectB);

			Assert.AreEqual(2, bFields.Count());

			Assert.True(_vrpFieldValidator.AppliesTo(_resourcePathTestObjectB));
			Assert.False(_vrpFieldValidator.Validate(_resourcePathTestObjectB));
		}

		private class ResourcePathTestObjectA : MonoBehaviour
		{
			[ResourcePath]
			public string resourcePath;

			[ResourcePath]
			public ResourcePathHelper helper;
		}

		private class ResourcePathTestObjectB : ScriptableObject
		{
			[ResourcePath]
			public string resourcePath;

			[ResourcePath]
			public ResourcePathHelper helper;
		}

		private class ResourcePathHelper
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

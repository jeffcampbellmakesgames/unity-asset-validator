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
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// ReSharper disable UnusedMember.Global

namespace JCMG.AssetValidator.Editor.Tests
{
	[TestFixture]
	#pragma warning disable S2187 // TestCases should contain tests
	public class SceneValidatorTests
	#pragma warning restore S2187 // TestCases should contain tests
	{
		private ActiveSceneValidatorManager _sValidatorManager;
		private ClassTypeCache _coreCache;
		private LogCache _logCache;

		[SetUp]
		public void Setup()
		{
			_coreCache = new ClassTypeCache();
			_coreCache.AddTypeWithAttribute<MonoBehaviourTwo, ValidateAttribute>();
			_logCache = new LogCache();

			_sValidatorManager = new ActiveSceneValidatorManager(_coreCache, _logCache);
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

		#pragma warning disable 0649

		private class SceneParserTestAttribute : AssetValidator.FieldAttribute
		{
		}

		[OnlyIncludeInTests]
		[FieldValidator("scene_parser_test_validator", typeof(SceneParserTestAttribute))]
		private sealed class SceneParserTestValidator : FieldValidatorBase
		{
			public override bool Validate(Object obj)
			{
				return obj.GetType() != typeof(SceneParserTestObjectA);
			}
		}

		[Validate]
		private class SceneParserTestObjectA : MonoBehaviourTwo
		{
			[SceneParserTest]
			public object obj;
		}

		[Validate]
		private class SceneParserTestObjectB : MonoBehaviourTwo
		{

		}

		[Validate]
		private class SceneParserTestObjectC : MonoBehaviourTwo
		{
			[SceneParserTest]
			public object obj;
		}

		#pragma warning restore 0649
	}
}

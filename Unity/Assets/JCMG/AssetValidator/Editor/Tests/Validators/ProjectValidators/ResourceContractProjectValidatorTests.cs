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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace JCMG.AssetValidator.Editor.Tests
{
	[TestFixture]
	public class ResourceContractProjectValidatorTests
	{
		private ResourceContractProjectValidator _cValidator;

		[SetUp]
		public void SetUp()
		{
			_cValidator = new ResourceContractProjectValidator(useTestContracts: true);
		}

		[Test]
		[Ignore(
			"In order to include the unit tests, but not have unnecessary Resources assets included with this project I have " +
			"set this test to be ignored and renamed the Resources folder it was using for the unit test to 'Resource_Unit_Test'." +
			"In order to run this unit test, comment out this ignore attribute and rename that folder to Resources.")]
		public void AssertThatUnitTestGuaranteeIsFoundAndCanBeValidated()
		{
			_cValidator.Search();

			var flags = BindingFlags.Instance | BindingFlags.NonPublic;
			var prop = typeof(ResourceContractProjectValidator).GetProperty("ResourcePaths", flags);

			Assert.IsNotNull(prop);

			var list = prop.GetValue(_cValidator, null) as List<string>;

			Assert.IsNotNull(list);

			list = list.Where(x => x == UnitTestResourcePathContract.UNIT_TEST_RESOURCE_PATH).ToList();
			prop.SetValue(_cValidator, list, null);

			Assert.True(_cValidator.GetNumberOfResults() == 1, "");
			Assert.True(_cValidator.Validate());
		}

		[OnlyIncludeInTests]
		public sealed class UnitTestResourcePathContract : ResourcePathContract
		{
			public const string UNIT_TEST_RESOURCE_PATH = "ProjectPrefabSearchTest";

			public override IEnumerable<string> GetPaths()
			{
				return new[] {UNIT_TEST_RESOURCE_PATH};
			}
		}
	}
}

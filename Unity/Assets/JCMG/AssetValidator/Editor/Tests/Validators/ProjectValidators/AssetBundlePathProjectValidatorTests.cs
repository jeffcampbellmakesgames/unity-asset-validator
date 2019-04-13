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
	public class AssetBundlePathProjectValidatorTests
	{
		private AssetBundlePathProjectValidator _aValidator;

		[SetUp]
		public void SetUp()
		{
			_aValidator = new AssetBundlePathProjectValidator(useTestContracts: true);
		}

		[Test]
		[Ignore("These tests require that an asset bundle be created with the name 'unit_test_bundle' and " +
		        "a scene be created in it with the name of 'bundle_item.unity'. I would prefer to not include" +
		        "any AssetBundles in the project by default and only create these when needing to run these unit tests.")]
		public void AssetThatValidatorCanFindAndValidateUnitTestBundle()
		{
			_aValidator.Search();

			var flags = BindingFlags.Instance | BindingFlags.NonPublic;
			var prop = typeof(AssetBundlePathProjectValidator).GetProperty("AssetBundleValidationCache", flags);

			Assert.IsNotNull(prop);

			var dict = prop.GetValue(_aValidator, null) as Dictionary<string, List<string>>;

			Assert.IsNotNull(dict);

			dict = dict.Where(x => x.Key == UnitTestAssetBundleContract.UNIT_TEST_ASSET_VALID_BUNDLE)
				.ToDictionary(x => x.Key, x => x.Value);
			prop.SetValue(_aValidator, dict, null);

			Assert.True(dict.Count == 1);
			Assert.True(_aValidator.Validate());
		}

		[Test]
		[Ignore("These tests require that an asset bundle be created with the name 'unit_test_bundle' and " +
		        "a scene be created in it with the name of 'bundle_item.unity'. I would prefer to not include" +
		        "any AssetBundles in the project by default and only create these when needing to run these unit tests.")]
		public void AssetThatValidatorCannotFindAndValidateInvalidUnitTestBundle()
		{
			_aValidator.Search();

			var flags = BindingFlags.Instance | BindingFlags.NonPublic;
			var prop = typeof(AssetBundlePathProjectValidator).GetProperty("AssetBundleValidationCache", flags);

			Assert.IsNotNull(prop);

			var dict = prop.GetValue(_aValidator, null) as Dictionary<string, List<string>>;

			Assert.IsNotNull(dict);

			dict = dict.Where(x => x.Key == UnitTestAssetBundleContract.UNIT_TEST_ASSET_INVALID_BUNDLE)
				.ToDictionary(x => x.Key, x => x.Value);
			prop.SetValue(_aValidator, dict, null);

			Assert.True(dict.Count == 1);
			Assert.False(_aValidator.Validate());
		}

		[OnlyIncludeInTests]
		public sealed class UnitTestAssetBundleContract : AssetBundlePathContract
		{
			public const string UNIT_TEST_ASSET_VALID_BUNDLE = "unit_test_bundle";
			public const string UNIT_TEST_ASSET_BUNDLE_VALID_ITEM = "bundle_item.unity";

			public const string UNIT_TEST_ASSET_INVALID_BUNDLE = "invalid_unit_test_bundle";

			public const string UNIT_TEST_ASSET_BUNDLE_INVALID_ITEM = "invalid_bundle_item.unity";

			public override Dictionary<string, List<string>> GetPaths()
			{
				var dict = new Dictionary<string, List<string>>
				{
					{UNIT_TEST_ASSET_VALID_BUNDLE, new List<string> {UNIT_TEST_ASSET_BUNDLE_VALID_ITEM}},
					{UNIT_TEST_ASSET_INVALID_BUNDLE, new List<string> {UNIT_TEST_ASSET_BUNDLE_INVALID_ITEM}}
				};

				return dict;
			}
		}
	}
}

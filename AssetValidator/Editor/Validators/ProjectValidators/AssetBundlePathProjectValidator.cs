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
using UnityEditor;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="AssetBundlePathProjectValidator"/> is a project validator that searches for all
	/// <see cref="AssetBundlePathContract"/> derived classes and validates that the asset bundle paths
	/// gathered from them resolve to a non-null asset.
	/// </summary>
	[Validator("project_asset_bundle_path_validator")]
	[ValidatorDescription(
		"Searches the project for all subclasses of AssetBundlePathContract and ensures that there is a file " +
		"present at all bundle and bundle item paths acquired using the GetPath method on those contracts.")]
	[ValidatorExample(@"
public enum HeroClass
{
    Warrior,
    Rogue,
    Sorcerer
}

/// <summary>
/// This example uses static data (in this case an enum of hero class types) to determine
/// which icons should be in a particular bundle and which name they are expected to be in.
/// If they are not present, a validation error will be created with the details of the file
/// it could not find.
/// </summary>
public class HeroClassAssetBundleContract : AssetBundlePathContract
{
    public override Dictionary<string, List<string>> GetPaths()
    {
        var heroValues = (HeroClass[]) Enum.GetValues(typeof(HeroClass));
        var dict = new Dictionary<string, List<string>>();
        var bundleName = ""hero_icon"";
        dict.Add(bundleName, new List<string>());

        foreach (var heroValue in heroValues)
            dict[bundleName].Add(string.Format(""{0}_icon.png"", heroValue.ToString().ToLower()));

        return dict;
    }
}
")]
	public sealed class AssetBundlePathProjectValidator : ProjectValidatorBase
	{
		/// 'set' required here for unit tests
		// ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
		private Dictionary<string, List<string>> AssetBundleValidationCache { get; set; }

		private readonly bool _useTestContracts;

		private const string MissingAssetBundleWarning =
			"Could not find asset for bundle [{0}] with asset name of [{1}].";

		/// <summary>
		/// Empty constructor used for reflection.
		/// </summary>
		public AssetBundlePathProjectValidator()
		{
			AssetBundleValidationCache = new Dictionary<string, List<string>>();
		}

		/// <summary>
		/// Constructor used for unit tests
		/// </summary>
		/// <param name="useTestContracts"></param>
		public AssetBundlePathProjectValidator(bool useTestContracts)
		{
			AssetBundleValidationCache = new Dictionary<string, List<string>>();

			_useTestContracts = useTestContracts;
		}

		public override int GetNumberOfResults()
		{
			return AssetBundleValidationCache.Sum(x => x.Value.Count);
		}

		public override void Search()
		{
			if (AssetBundleValidationCache.Count > 0)
			{
				return;
			}

			var contracts = ReflectionTools.GetAllDerivedInstancesOfType<AssetBundlePathContract>();
			foreach (var contract in contracts)
			{
				if (!_useTestContracts &&
				    contract.GetType().GetCustomAttributes(typeof(OnlyIncludeInTestsAttribute), true).Length > 0)
				{
					continue;
				}

				var dict = contract.GetPaths();
				foreach (var kvp in dict)
				{
					if (AssetBundleValidationCache.ContainsKey(kvp.Key))
					{
						// TODO Iterate through the existing bundle contents and add any bundle items not present
					}
					else
					{
						AssetBundleValidationCache.Add(kvp.Key, kvp.Value);
					}
				}
			}
		}

		public override bool Validate()
		{
			var allPathsValidated = true;
			foreach (var assetBundle in AssetBundleValidationCache)
			{
				var validatedAssetBundleName = assetBundle.Key;
				var validatedAssetBundleContents = assetBundle.Value;
				var assetBundleContents =
					new List<string>(AssetDatabase.GetAssetPathsFromAssetBundle(validatedAssetBundleName));

				FileTools.ReduceAssetPathsToFileNames(assetBundleContents);

				for (var i = 0; i < validatedAssetBundleContents.Count; i++)
				{
					if (assetBundleContents.Contains(validatedAssetBundleContents[i]))
					{
						continue;
					}

					allPathsValidated = false;
					DispatchLogEvent(
						null,
						LogType.Error,
						string.Format(
							MissingAssetBundleWarning,
							validatedAssetBundleName,
							validatedAssetBundleContents[i]));
				}
			}

			return allPathsValidated;
		}
	}
}

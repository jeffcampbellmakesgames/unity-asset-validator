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
using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="AssetProjectValidatorBase"/> is an abstract project validator class that can be used to
	/// validate assets with a specific file extension. Derived classes should override <see cref="GetApplicableFileExtensions"/>
	/// to filter the appropriate files by way of extension and <see cref="ValidateObject"/> to inspect any
	/// discovered assets.
	/// </summary>
	[ValidatorDescription("AssetProjectValidatorBase should be derived from when there are specific file types " +
	                      "or extensions in the project that need to be validated.")]
	[ValidatorExample(@"
/// <summary>
/// This is an example of an AssetProjectValidatorBase that targets all files with a file extension of ""unity"".
/// When this validator is fired, it will find all files of that extension and attempt to load them using
/// AssetDatabase.Load and pass the loaded object and the path to ValidateObject to see whether or not
/// the asset passes validation or not. As validators of any type are an editor only feature, they
/// must be placed into an Editor folder.
/// </summary>
[ValidatorTarget(""test_project_asset_ext_validator"")]
public class NoWhiteSpaceInSceneNamesAssetProjectValidator : AssetProjectValidatorBase
{
    protected override string[] GetApplicableFileExtensions()
    {
        return new[] {""unity""};
    }

    protected override bool ValidateObject(Object assetObj, string path)
    {
        var sceneName = path.Split('/').Last();

        if (!sceneName.Contains("" "")) return true;

        DispatchLogEvent(assetObj,
                          logType.Error,
                          string.Format(""Scene [{0}] should not have any whitespace in its name"",
                                        sceneName));

        return false;
    }
}
")]
	public abstract class AssetProjectValidatorBase : ProjectValidatorBase
	{
		protected List<string> _assetPaths;
		protected bool _successfullyValidated;

		protected AssetProjectValidatorBase()
		{
			_assetPaths = new List<string>();
			_successfullyValidated = true;
		}

		public sealed override int GetNumberOfResults()
		{
			return _assetPaths.Count;
		}

		public sealed override void Search()
		{
			var extensions = GetApplicableFileExtensions();

			_assetPaths.AddRange(FileTools.GetUnityFilePaths(extensions));
		}

		/// <summary>
		/// Override this in subclasses of AssetProjectValidatorBase to return an array of file extensions with
		/// or without a period at the beginning. All files of that extension will be loaded one by one using
		/// AssetDatabase.LoadAssetAtPath and passed to ValidateObject in order to validate.
		/// </summary>
		/// <returns></returns>
		protected virtual string[] GetApplicableFileExtensions()
		{
			throw new NotImplementedException();
		}

		public sealed override bool Validate()
		{
			for (var i = 0; i < _assetPaths.Count; i++)
			{
				var obj = AssetDatabase.LoadAssetAtPath<Object>(_assetPaths[i]);
				_successfullyValidated &= ValidateObject(obj, _assetPaths[i]);
			}

			return _successfullyValidated;
		}

		/// <summary>
		/// Returns true on successful validation of the object, otherwise false
		/// </summary>
		/// <param name="assetObj">The object loaded by the AssetDatabase at this path.</param>
		/// <param name="path">The unity path at which this asset can be found.</param>
		/// <returns></returns>
		protected virtual bool ValidateObject(Object assetObj, string path)
		{
			throw new NotImplementedException();
		}
	}
}

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
using System.Linq;
using UnityEngine;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// Helper methods for running the AssetValidator as part of a continuous integration setup either
	/// from Unit Tests or as part of a build job.
	/// </summary>
	public static class ContinuousIntegrationTools
	{
		/// <summary>
		/// Represents the result of a validation run.
		/// </summary>
		public class Result
		{
			/// <summary>
			/// Returns true if zero validation errors were found, false otherwise.
			/// </summary>
			public bool isSuccessful { get; internal set; }

			/// <summary>
			/// The message describing the result of the validation run.
			/// </summary>
			public string message { get; internal set; }
		}

		// Arguments
		private const string VALIDATE_PROJECT_ASSETS = "validateprojectassets";
		private const string VALIDATE_CROSS_SCENES = "validatecrossscenes";
		private const string SCENE_VALIDATE_MODE_KEY = "scenevalidationmode";
		private const string OUTPUT_FORMAT_KEY = "outputformat";

		// Console Messages
		private const string FileOutputFormatNotFoundWarning =
			"No FileOutputFormat has been specified and so none will be written/displayed.";
		private const string ArgumentErrorFormat =
			"Could not run validation as the argument {0} for [{1}] could not be parsed";
		private const string ValidationRunMessageFormat = "SceneValidationMode: [{0}], OutputMode: [{1}], " +
		                                            "DoValidateProjectAssets: [{2}], DoValidateCrossScenes: " +
		                                            "[{3}], Filename: [{4}]";
		private const string ValidationSuccessMessage = "No AssetValidation errors were found.";
		private const string ValidationFailedMessage = "Several AssetValidation errors were found";
		private const string ValidationErrorFormat =
			"AssetValidation failed to be run with the following exception: [{0}]";

		private const char ArgumentDelimiter = ':';

		/// <summary>
		/// Runs validation using arguments passed to the commandline.
		/// </summary>
		public static void Run()
		{
			var doValidateProjectAssets = false;
			var doValidateCrossScenes = false;
			var sceneValidationMode = SceneValidationMode.None;
			var outputMode = FileOutputFormat.None;
			var fileName = string.Empty;
			var argsDict = CommandLineTools.GetNamedCommandlineArguments(ArgumentDelimiter);

			// Parse whether or not to validate assets in the project
			if (argsDict.ContainsKey(VALIDATE_PROJECT_ASSETS) &&
			    !bool.TryParse(argsDict[VALIDATE_PROJECT_ASSETS], out doValidateProjectAssets))
			{
				LogArgumentError(VALIDATE_PROJECT_ASSETS, argsDict[VALIDATE_PROJECT_ASSETS]);
			}

			// Parse whether or not to validate across scenes in the project
			if (argsDict.ContainsKey(VALIDATE_CROSS_SCENES) &&
			    !bool.TryParse(argsDict[VALIDATE_CROSS_SCENES], out doValidateCrossScenes))
			{
				LogArgumentError(VALIDATE_CROSS_SCENES, argsDict[VALIDATE_CROSS_SCENES]);
			}

			// Parse Scene Validation Mode
			if (argsDict.ContainsKey(SCENE_VALIDATE_MODE_KEY))
			{
				try
				{
					sceneValidationMode = (SceneValidationMode)Enum.Parse(typeof(SceneValidationMode),
						argsDict[SCENE_VALIDATE_MODE_KEY], true);
				}
				catch (Exception)
				{
					LogArgumentError(argsDict[SCENE_VALIDATE_MODE_KEY], SCENE_VALIDATE_MODE_KEY);
					return;
				}
			}

			// Parse Output Format
			if (argsDict.ContainsKey(OUTPUT_FORMAT_KEY))
			{
				try
				{
					outputMode = (FileOutputFormat)Enum.Parse(typeof(FileOutputFormat), argsDict[OUTPUT_FORMAT_KEY], true);
				}
				catch (Exception)
				{
					LogArgumentError(argsDict[SCENE_VALIDATE_MODE_KEY], SCENE_VALIDATE_MODE_KEY);
					return;
				}
			}
			else
			{
				Debug.LogWarning(FileOutputFormatNotFoundWarning);
			}

			Debug.LogFormat(ValidationRunMessageFormat,
				sceneValidationMode,
				outputMode,
				doValidateProjectAssets,
				doValidateCrossScenes,
				fileName);

			var result = RunValidation(sceneValidationMode,
				outputMode,
				doValidateProjectAssets,
				doValidateCrossScenes,
				fileName);

			if (result.isSuccessful)
			{
				Debug.Log(result.message);
			}
			else
			{
				Debug.LogError(result.message);
			}
		}

		/// <summary>
		/// Logs an error for a passed command-line argument.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		private static void LogArgumentError(string key, string value)
		{
			Debug.LogFormat(ArgumentErrorFormat, key, value);
		}

		/// <summary>
		/// Runs validation against the project in <see cref="SceneValidationMode"/>
		/// <paramref name="validationMode"/> and writes the log file to a file with the default name.
		/// </summary>
		/// <param name="validationMode">The <see cref="SceneValidationMode"/> the validation is run in.</param>
		/// <param name="fileOutputFormat">The <see cref="FileOutputFormat"/> the file will be written in, if any.</param>
		/// <param name="doValidateProjectAssets">True if project assets should be validated, false if not.</param>
		/// <param name="doValidateAcrossScenes">True if cross-scene validation should be performed.</param>
		/// <returns></returns>
		public static Result RunValidation(
			SceneValidationMode validationMode,
			FileOutputFormat fileOutputFormat,
			bool doValidateProjectAssets,
			bool doValidateAcrossScenes)
		{
			return RunValidation(
				validationMode,
				fileOutputFormat,
				doValidateProjectAssets,
				doValidateAcrossScenes,
				string.Empty);
		}

		/// <summary>
		/// Runs validation against the project in <see cref="SceneValidationMode"/>
		/// <paramref name="validationMode"/> and writes the log file to a file with a custom name.
		/// </summary>
		/// <param name="validationMode">The <see cref="SceneValidationMode"/> the validation is run in.</param>
		/// <param name="fileOutputFormat">The <see cref="FileOutputFormat"/> the file will be written in, if any.</param>
		/// <param name="doValidateProjectAssets">True if project assets should be validated, false if not.</param>
		/// <param name="doValidateAcrossScenes">True if cross-scene validation should be performed.</param>
		/// <param name="fileName">A custom filename for the log results to be written to.</param>
		/// <returns></returns>
		public static Result RunValidation(
			SceneValidationMode validationMode,
			FileOutputFormat fileOutputFormat,
			bool doValidateProjectAssets,
			bool doValidateAcrossScenes,
			string fileName)
		{
			try
			{
				var logCache = new LogCache();
				var assetValidationRunner = new AssetValidatorRunner(logCache, validationMode);
				if (doValidateProjectAssets)
				{
					assetValidationRunner.EnableProjectAssetValidation();
				}

				if (doValidateAcrossScenes)
				{
					assetValidationRunner.EnableCrossSceneValidation();
				}

				assetValidationRunner.Run();

				var newFileName = string.IsNullOrEmpty(fileName)
					? EditorConstants.DefaultLogFilename
					: fileName;

				LogFileWriter.WriteLogs(newFileName, fileOutputFormat, logCache);

				var result = new Result
				{
					isSuccessful = logCache.All(x => x.logType != LogType.Error)
				};
				result.message = result.isSuccessful
					? ValidationSuccessMessage
					: ValidationFailedMessage;

				return result;
			}
			catch (Exception ex)
			{
				return new Result
				{
					isSuccessful = false,
					message = string.Format(ValidationErrorFormat, ex)
				};
			}
		}
	}
}

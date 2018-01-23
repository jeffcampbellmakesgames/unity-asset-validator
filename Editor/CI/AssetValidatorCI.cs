/*
AssetValidator 
Copyright (c) 2018 Jeff Campbell

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
using JCMG.AssetValidator.Editor.Utility;
using JCMG.AssetValidator.Editor.Validators;
using JCMG.AssetValidator.Editor.Validators.Output;
using JCMG.AssetValidator.Editor.Window;
using System;
using System.Linq;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.CI
{
    /// <summary>
    /// Helper methods for running the AssetValidator as part of a continuous integration setup either from Unit Tests
    /// or as part of a build job.
    /// </summary>
    public static class AssetValidatorCI
    {
        private const string VALIDATE_PROJECT_ASSETS = "validateprojectassets";
        private const string VALIDATE_CROSS_SCENES = "validatecrossscenes";
        private const string SCENE_VALIDATE_MODE_KEY = "scenevalidationmode";
        private const string OUTPUT_FORMAT_KEY = "outputformat";
        private const string FILENAME_MODE_KEY = "filename";

        public static void Run()
        {
            var doValidateProjectAssets = false;
            var doValidateCrossScenes = false;
            var sceneValidationMode = SceneValidationMode.None;
            var outputMode = OutputFormat.None;
            var fileName = "";
            var argsDict = CommandLineUtility.GetNamedCommandlineArguments(':');

            // Parse whether or not to validate assets in the project
            if (argsDict.ContainsKey(VALIDATE_PROJECT_ASSETS))
            {
                if(!bool.TryParse(argsDict[VALIDATE_PROJECT_ASSETS], out doValidateProjectAssets))
                    LogArgumentError(VALIDATE_PROJECT_ASSETS, argsDict[VALIDATE_PROJECT_ASSETS]);
            }

            // Parse whether or not to validate across scenes in the project
            if (argsDict.ContainsKey(VALIDATE_CROSS_SCENES))
            {
                if (!bool.TryParse(argsDict[VALIDATE_CROSS_SCENES], out doValidateCrossScenes))
                    LogArgumentError(VALIDATE_CROSS_SCENES, argsDict[VALIDATE_CROSS_SCENES]);
            }

            // Parse Scene Validation Mode
            if (argsDict.ContainsKey(SCENE_VALIDATE_MODE_KEY))
            {
                try
                {
                    sceneValidationMode = (SceneValidationMode)Enum.Parse(typeof(SceneValidationMode), argsDict[SCENE_VALIDATE_MODE_KEY], true);
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
                    outputMode = (OutputFormat)Enum.Parse(typeof(OutputFormat), argsDict[OUTPUT_FORMAT_KEY], true);
                }
                catch (Exception)
                {
                    LogArgumentError(argsDict[SCENE_VALIDATE_MODE_KEY], SCENE_VALIDATE_MODE_KEY);
                    return;
                }
            }
            else
            {
                Debug.LogWarning("No OutputFormat has been specified and so none will be written/displayed.");
            }

            // Parse OutputFilename
            if (argsDict.ContainsKey(FILENAME_MODE_KEY))
            {
                fileName = argsDict[FILENAME_MODE_KEY];
                // TODO Fix This
                //if (!FileUtility.IsValidFilename(fileName))
                //{
                //    Debug.LogFormat("{0} is not a valid filename, please check for any illegal characters for this operating system", fileName);
                //    return;
                //}
            }

            Debug.LogFormat("SceneValidationMode: [{0}], OutputMode: [{1}], DoValidateProjectAssets: [{2}], DoValidateCrossScenes: [{3}], Filename: [{4}]", 
                            sceneValidationMode, outputMode, doValidateProjectAssets, doValidateCrossScenes, fileName);

            RunValidation(sceneValidationMode,
                          outputMode,
                          doValidateProjectAssets,
                          doValidateCrossScenes,
                          fileName);
        }

        private static void LogArgumentError(string key, string value)
        {
            Debug.LogFormat("Could not run validation as the argument {0} for [{1}] could not be parsed", key, value);
        }

        public class Result
        {
            /// <summary>
            /// Returns true if zero validation errors were found, false otherwise.
            /// </summary>
            public bool isSuccessful;

            /// <summary>
            /// The message describing the result of the validation run.
            /// </summary>
            public string message;
        }

        public static Result RunValidation(SceneValidationMode vMode,
                                           OutputFormat oFormat,
                                           bool doValidateProjectAssets = false,
                                           bool doValidateAcrossScenes = false,
                                           string fileName = "asset_validator_results")
        {
            AssetValidatorUtility.EditorOuputFormat = oFormat;
            AssetValidatorUtility.EditorFilename = fileName;

            try
            {
                var assetLogger = new AssetValidatorLogger();
                using (var assetValidationRunner = new AssetValidatorRunner(assetLogger, vMode))
                {
                    if (doValidateProjectAssets)
                        assetValidationRunner.EnableProjectAssetValidation();

                    if (doValidateAcrossScenes)
                        assetValidationRunner.EnableCrossSceneValidation();

                    assetValidationRunner.Run();
                }

                var logs = assetLogger.GetLogs();
                if (AssetValidatorUtility.EditorOuputFormat != OutputFormat.None)
                {
                    using (var writer = new AssetValidatorLogWriter(AssetValidatorUtility.EditorFilename,
                        AssetValidatorUtility.EditorOuputFormat))
                    {
                        if(AssetValidatorUtility.EditorOuputFormat == OutputFormat.Html)
                            writer.CreateHtmlStyles(logs);

                        writer.AppendHeader();
                        for (var i = 0; i < logs.Count; i++)
                            writer.AppendVLog(logs[i]);

                        writer.AppendFooter();
                        writer.Flush();
                    }
                }

                var result = new Result { isSuccessful = logs.Any(x => x.vLogType == VLogType.Error) };
                result.message = result.isSuccessful
                    ? "No AssetValidation errors were found."
                    : "Several AssetValidation errors were found";

                return result;
            }
            catch (Exception ex)
            {
                return new Result()
                {
                    isSuccessful = false,
                    message = string.Format("AssetValidation failed to be run with the following exception: [{0}]", ex)
                };
            }
        }
    }
}

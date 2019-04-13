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
namespace JCMG.AssetValidator.Editor
{
	internal static class EditorConstants
	{
		// AssetValidatorEditorWindow
		public const string EditorWindowTitle = "Asset Validator";

		// Continue Dialog Window
		public const string ContinuePromptMessage =
			"Continuing with validation may unload the current scene. Do you wish to continue?";
		public const string CancelButtonText = "Cancel";
		public const string ProceedValidationButtonText = "Continue Validation";

		// AssetValidatorOverrideConfig
		public const string OverrideConfigName = "AssetValidatorOverrideConfig";
		public const string OverrideConfigToString = "Validator with Symbol [{0}] is Enabled: [{1}]";

		// AssetValidatorOverrideConfig Inspector
		public const string OverrideConfigEnabledHeader = "Is Enabled";
		public const string OverrideConfigClassHeader = "Validator Class";
		public const string OverrideConfigTypeHeader = "Validator Type";

		// Validator Types
		public const string ObjectValidatorTypeName = "Object Validator";
		public const string FieldValidatorTypeName = "Field Validator";
		public const string CrossSceneValidatorTypeName = "Cross Scene Validator";
		public const string ProjectValidatorTypeName = "Project Validator";
		public const string UnknownValidatorTypeName = "Unknown Validator";

		// Scene Validation Mode Types
		public const string ValidationModeNone = "None";
		public const string ValidationModeActive = "Active Scene Only";
		public const string ValidationModeAllScenes = "All Scenes";
		public const string ValidationModeAllBuildScenes = "All Build Scenes";
		public const string ValidationModeAllBuildAndAssetBundleScenes = "All Build and Asset Bundle Scenes";

		// Messages
		public const string ValidationHasCompletedMessage = "AssetValidatorRunner has completed...";

		// Warnings
		public const string CouldNotFindConfigWarning =
			"Could not find an AssetValidatorOverrideConfig named AssetValidatorOverrideConfig at the root of " +
			"a Resources folder. Overriding will be ignored in its absence.";

		public const string ConfigValidatorDisabledWarning =
			"Validator of type [{0}] is disabled in the AssetValidatorOverrideConfig at [{1}]";

		public const string ConfigAssetNotInResourcesPath =
			"This asset must be located at the root of a Resources folder in order for " +
			"it to be loadable and used by the AssetValidator. Otherwise it will be ignored.";

		public const string InvalidTypeWarning = "Generic type T must be an enum.";

		public const string CouldNotPingObjectWarning = "Could not find object at path of [{0}]";

		// Errors
		public const string OutputFormatIsInvalid = "FileOutputFormat [{0}] is not valid!";

		// TreeView Group Modes
		public const string GroupByValidator = "Group by Validator";
		public const string GroupBySource = "Group by Source (Scene and Project)";

		// Tree View UI
		public const string TreeViewAreaLabel = "Area:   ";
		public const string TreeViewObjectLabel = "Object:   ";
		public const string TreeViewMessageLabel = "Message:   ";

		public const string PingObjectButtonText = "Ping Object";
		public const string LoadSceneButtonText = "Load Scene";

		// GameObject Lifecycle
		public const string NullGameObject = "null";

		// File-related
		public const string WildcardFilterFormat = "*.{0}";
		public const string PeriodStr = ".";

		public const string ForwardSlashStr = "/";
		public const char ForwardSlashChar = '/';

		public const string BackSlashEscapedStr = "\\";

		public const string RelativePathFormat = "/{0}";

		// Unity Magic Folder Names
		public const string AssetsFolderName = "Assets";
		public const string ResourcesFolderName = "resources";

		// ValidationLog Constants
		public const string ProjectDescription = "Project";
		public const string NoneDescription = "None";

		// Filename
		public const string DefaultLogFilename = "asset_validator_results";
	}
}

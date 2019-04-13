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
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JCMG.AssetValidator.Editor
{
	internal sealed class AssetValidatorEditorWindow : EditorWindow
	{
		private LogCache _logCache;
		private AssetValidatorRunner _runner;

		// EditorUX
		private Rect _logRect;
		private const float FIXED_LEFT_COLUMN_WIDTH = 110f;
		private GUILayoutOption[] _guiLayoutOptions;

		///  Group By Options
		private int _groupByOptionsIndex;

		private LogGroupingMode[] _groupByOptionsValues;
		private string[] _groupByOptionsNames;

		/// File Logging Output Options
		private int _outputFormatOptionsIndex;
		private FileOutputFormat _selectedFileOutputFormat;
		private FileOutputFormat[] _fileOutputFormatOptions;
		private string[] _outputFormatOptionNames;
		private string _outputFilename;

		// Scene Validation Options
		private int _sceneValidationIndex;
		private SceneValidationMode[] _sceneValidationModeOptions;
		private string[] _sceneValidationModeOptionNames;

		// Project Options
		private int _projectSelectionIndex;

		private ValidationLogTreeView _validationLogTreeView;

		private readonly string[] _projectOptions = {
			"Validate Project Assets ON",
			"Validate Project Assets OFF"
		};

		// Debug Options
		private int _debugSelectionIndex;

		private readonly string[] _debugOptions = {
			"Debugging OFF",
			"Debugging ON"
		};

		[MenuItem("Tools/AssetValidator/Open Asset Validator Window", false, priority = 0)]
		[MenuItem("Window/Asset Validator")]
		public static void LaunchWindow()
		{
			var window = GetWindow<AssetValidatorEditorWindow>();
			window.Show();
		}

		public static void LaunchWindowWithValidation(
			SceneValidationMode validationMode,
			FileOutputFormat fileOutputFormat)
		{
			var window = GetWindow<AssetValidatorEditorWindow>();
			window.Show();
			window.LaunchValidator(validationMode, fileOutputFormat, false, false, string.Empty);
		}

		/// <summary>
		/// Launch Validator is a one-all fit for being able to run Validation
		/// </summary>
		/// <param name="vMode"></param>
		/// <param name="fileOutputFormat"></param>
		/// <param name="doValidateProjectAssets"></param>
		/// <param name="doValidateAcrossScenes"></param>
		/// <param name="fileName"></param>
		private void LaunchValidator(SceneValidationMode vMode,
			FileOutputFormat fileOutputFormat,
			bool doValidateProjectAssets,
			bool doValidateAcrossScenes,
			string fileName)
		{
			_selectedFileOutputFormat = fileOutputFormat;
			_outputFilename = string.IsNullOrEmpty(fileName)
				? EditorConstants.DefaultLogFilename
				: fileName;

			OnValidateSelectionClick(vMode, doValidateProjectAssets, doValidateAcrossScenes);
		}

		private void OnEnable()
		{
			titleContent.text = EditorConstants.EditorWindowTitle;

			_logCache = new LogCache();

			_guiLayoutOptions = new[] { GUILayout.Height(30f), GUILayout.MinWidth(625f) };

			_groupByOptionsValues = (LogGroupingMode[])Enum.GetValues(typeof(LogGroupingMode));
			_groupByOptionsNames = _groupByOptionsValues.Select(ReflectionTools.GetEnumString).ToArray();

			_fileOutputFormatOptions = (FileOutputFormat[])Enum.GetValues(typeof(FileOutputFormat));
			_outputFormatOptionNames = _fileOutputFormatOptions.Select(ReflectionTools.GetEnumString).ToArray();

			_sceneValidationModeOptions = (SceneValidationMode[])Enum.GetValues(typeof(SceneValidationMode));
			_sceneValidationModeOptionNames = _sceneValidationModeOptions.Select(ReflectionTools.GetEnumString).ToArray();

			_validationLogTreeView = new ValidationLogTreeView(new TreeViewState());

			EditorSceneManager.sceneOpened += OnSceneLoaded;
		}

		private void OnDisable()
		{
			EditorSceneManager.sceneOpened -= OnSceneLoaded;
		}

		private void OnGUI()
		{
			// Create any needed local vars
			EditorGUI.BeginDisabledGroup(_runner != null && _runner.IsRunning());

			// Configuration Header
			EditorGUILayout.LabelField("Configuration");
			EditorGUILayout.Separator();

			// Project Options
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Project Options", EditorStyles.boldLabel,
				GUILayout.Width(FIXED_LEFT_COLUMN_WIDTH));
			_projectSelectionIndex = GUILayout.Toolbar(
				_projectSelectionIndex,
				_projectOptions,
				_guiLayoutOptions);
			EditorGUILayout.EndHorizontal();

			// SceneValidationMode Options
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Scene Options", EditorStyles.boldLabel,
				GUILayout.Width(FIXED_LEFT_COLUMN_WIDTH));
			_sceneValidationIndex = GUILayout.Toolbar(
				_sceneValidationIndex,
				_sceneValidationModeOptionNames,
				_guiLayoutOptions);
			EditorGUILayout.EndHorizontal();

			// Output Options
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Output Options", EditorStyles.boldLabel,
				GUILayout.Width(FIXED_LEFT_COLUMN_WIDTH));
			_outputFormatOptionsIndex = GUILayout.Toolbar(
				_outputFormatOptionsIndex,
				_outputFormatOptionNames,
				_guiLayoutOptions);
			_selectedFileOutputFormat = _fileOutputFormatOptions[_outputFormatOptionsIndex];
			EditorGUILayout.EndHorizontal();

			// Debug Options
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Debug Options", EditorStyles.boldLabel,
				GUILayout.Width(FIXED_LEFT_COLUMN_WIDTH));
			_debugSelectionIndex = GUILayout.Toolbar(_debugSelectionIndex, _debugOptions, _guiLayoutOptions);
			ProjectTools.IsDebugging = _debugSelectionIndex > 0;
			EditorGUILayout.EndHorizontal();

			// Run Validation Button
			// Configuration Header
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Execute");

			if (GUILayout.Button("Run Validation", GUILayout.Height(40f)))
			{
				OnValidateSelectionClick(
					GetSelectedSceneMode(),
					DoValidateProjectAssets(),
					DoValidateAcrossScenes());
			}

			EditorGUI.EndDisabledGroup();

			EditorGUILayout.Separator();

			// If we are running validators, show progress
			if (_runner != null && _runner.IsRunning())
			{
				var progressRect = EditorGUILayout.BeginVertical();
				EditorGUI.ProgressBar(progressRect, _runner.GetProgress(), _runner.GetProgressMessage());
				GUILayout.Space(16);
				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Validation Log Views");
			EditorGUILayout.Separator();

			// Log Grouping Options
			EditorGUI.BeginDisabledGroup(_runner != null && _runner.IsRunning());
			var groupByRect = EditorGUILayout.BeginHorizontal();
			_groupByOptionsIndex = GUILayout.Toolbar(
				_groupByOptionsIndex,
				_groupByOptionsNames,
				_guiLayoutOptions);
			EditorGUILayout.EndHorizontal();

			groupByRect.y += EditorGUIUtility.standardVerticalSpacing * 2f;
			_logRect = new Rect(
				groupByRect.xMin,
				groupByRect.yMax,
				groupByRect.width,
				position.height - groupByRect.yMin - 25f);

			// Display all logs for the current validation, if any
			EditorGUI.EndDisabledGroup();

			if (!HasLogs() || _runner != null && _runner.IsRunning())
			{
				EditorGUILayout.LabelField("No validation issues found...");
			}
			else if (!IsRunning() && HasLogs())
			{
				OnLogsGUI();
			}
		}

		private void Update()
		{
			if (_runner == null)
			{
				return;
			}

			if (!_runner.IsRunning())
			{
				return;
			}

			_runner.ContinueRunning();

			ForceEditorWindowUpdate();

			if (!_runner.IsComplete())
			{
				return;
			}

			if (ProjectTools.IsDebugging)
			{
				Debug.Log(EditorConstants.ValidationHasCompletedMessage);
			}

			ForceEditorWindowUpdate();

			LogFileWriter.WriteLogs(_outputFilename, _selectedFileOutputFormat, _logCache);
		}

		private SceneValidationMode GetSelectedSceneMode()
		{
			return _sceneValidationModeOptions[_sceneValidationIndex];
		}

		private bool DoValidateProjectAssets()
		{
			return _projectSelectionIndex == 0;
		}

		private bool DoValidateAcrossScenes()
		{
			return _sceneValidationModeOptions[_sceneValidationIndex] != SceneValidationMode.None;
		}

		private bool IsRunning()
		{
			return _runner != null && _runner.IsRunning();
		}

		private bool HasLogs()
		{
			return _logCache != null && _logCache.HasLogs();
		}

		private void ForceEditorWindowUpdate()
		{
			Repaint();
		}

		private void OnValidateSelectionClick(
			SceneValidationMode validationMode,
			bool doValidateProjectAssets = false,
			bool doValidateAcrossScenes = false)
		{
			// Only perform validation across multiple scenes if the current scene is saved, or
			// barring that if you don't care if it gets unloaded and lose changes
			var canExecute = true;
			var currentScene = SceneManager.GetActiveScene();
			if (currentScene.isDirty &&
			    validationMode != SceneValidationMode.None &&
			    validationMode != SceneValidationMode.ActiveScene)
			{
				var didSave = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() && !currentScene.isDirty;
				if (!didSave)
				{
					canExecute = EditorUtility.DisplayDialog(
						EditorConstants.EditorWindowTitle,
						EditorConstants.ContinuePromptMessage,
						EditorConstants.ProceedValidationButtonText,
						EditorConstants.CancelButtonText);
				}
			}

			if (!canExecute)
			{
				return;
			}

			// Clear out any remaining logs and queue up an AssetValidationRunner with
			// the desired config
			_logCache.ClearLogs();
			_runner = new AssetValidatorRunner(_logCache, validationMode);

			if (doValidateProjectAssets)
			{
				_runner.EnableProjectAssetValidation();
			}

			if (doValidateAcrossScenes)
			{
				_runner.EnableCrossSceneValidation();
			}
		}

		private void OnSceneLoaded(Scene scene, OpenSceneMode mode)
		{
			_validationLogTreeView.TryReload();
		}

		private void OnLogsGUI()
		{
			_validationLogTreeView.SetLogData(_logCache);
			_validationLogTreeView.SetGroupByMode(_groupByOptionsValues[_groupByOptionsIndex]);
			_validationLogTreeView.Reload();
			_validationLogTreeView.OnGUI(_logRect);
		}
	}
}

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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_5_6_OR_NEWER

using UnityEditor.IMGUI.Controls;
using UnityEngine.SceneManagement;

#endif

namespace JCMG.AssetValidator.Editor.Window
{
    public class AssetValidatorEditorWindow : EditorWindow, IDisposable
    {
        private AssetValidatorLogger _logger;
        private AssetValidatorRunner _runner;

        // EditorUX
        private GUISkin _originalGUISkin;
        private GUISkin _customGUISkin;
        private GUIStyle _toolbarStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _headerStyle;
        private Rect _groupByRect;
        private Rect _logRect;
        private const float FIXED_LEFT_COLUMN_WIDTH = 110f;

        ///  Group By Options
        private int _groupByOptionsIndex;
        private VLogTreeGroupByMode[] _groupByOptionsValues;
        private string[] _groupByOptionsNames;

        /// File Logging Output Options
        private int _outputFormatOptionsIndex;
        private OutputFormat[] _ouputFormatOptions;
        private string[] _outputFormatOptionNames;

        // Scene Validation Options
        private int _sceneValidationIndex;
        private SceneValidationMode[] _sceneValidationModeOptions;
        private string[] _sceneValidationModeOptionNames;

        // Project Options
        private int _projectSelectionIndex;
        private readonly string[] _projectOptions = new[]
        {
            "Validate Project Assets ON",
            "Validate Project Assets OFF"
        };

        // Debug Options
        private int _debugSelectionIndex;
        private readonly string[] _debugOptions = new[]
        {
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

        public static void LaunchWindowWithValidation(SceneValidationMode vMode,
                                                      OutputFormat oFormat,
                                                      bool doValidateProjectAssets = false,
                                                      bool doValidateAcrossScenes = false,
                                                      string fileName = "asset_validator_results")
        {
            var window = GetWindow<AssetValidatorEditorWindow>();
            window.Show();
            window.LaunchValidator(vMode, oFormat, doValidateProjectAssets, doValidateAcrossScenes, fileName);
        }

        private void OnEnable()
        {
            titleContent.text = "Asset Validator";

            _customGUISkin = GetGUISkin();
            _toolbarStyle = _customGUISkin.button;

            _logger = new AssetValidatorLogger();

            _groupByOptionsValues = (VLogTreeGroupByMode[])Enum.GetValues(typeof(VLogTreeGroupByMode));
            _groupByOptionsNames = _groupByOptionsValues.Select(x => ReflectionUtility.ToEnumString(x)).ToArray();

            _ouputFormatOptions = (OutputFormat[]) Enum.GetValues(typeof(OutputFormat));
            _outputFormatOptionNames = _ouputFormatOptions.Select(x => ReflectionUtility.ToEnumString(x)).ToArray();

            _sceneValidationModeOptions = (SceneValidationMode[]) Enum.GetValues(typeof(SceneValidationMode));
            _sceneValidationModeOptionNames = _sceneValidationModeOptions.Select(x => ReflectionUtility.ToEnumString(x)).ToArray();

            InitOnEnable();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private static GUISkin GetGUISkin()
        {
            return (GUISkin)Resources.Load("AssetValidatorSkin");
        }

        private static void LoadScenePath(VLog log)
        {
            EditorSceneManager.OpenScene(log.scenePath);
        }

        private void TryPingObject(VLog log)
        {
            Object obj = null;
            switch (log.source)
            {
                case VLogSource.Scene:
                    obj = GameObject.Find(log.objectPath);
                    break;
                case VLogSource.Project:
                    obj = AssetDatabase.LoadAssetAtPath(log.objectPath, typeof(Object));
                    break;
            }

            if (obj != null)
                EditorGUIUtility.PingObject(obj);
            else
                Debug.LogWarningFormat("Could not find object at path of [{0}]", log.objectPath);
        }

        private void OnGUI()
        {
            CacheGUIInfo();

            // Create any needed local vars
            var guiLayoutOptions = new [] { GUILayout.Height(30f), GUILayout.MinWidth(625f) };
            GUI.skin = _customGUISkin;

            EditorGUI.BeginDisabledGroup(_runner != null && _runner.IsRunning());

            // Configuration Header
            EditorGUILayout.LabelField("Configuration", _headerStyle);
            EditorGUILayout.Separator();

            // Project Options
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Project Options", EditorStyles.boldLabel, GUILayout.Width(FIXED_LEFT_COLUMN_WIDTH));
            _projectSelectionIndex = GUILayout.Toolbar(_projectSelectionIndex, _projectOptions, _toolbarStyle, guiLayoutOptions);
            EditorGUILayout.EndHorizontal();

            // SceneValidationMode Options
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scene Options", EditorStyles.boldLabel, GUILayout.Width(FIXED_LEFT_COLUMN_WIDTH));
            _sceneValidationIndex = GUILayout.Toolbar(_sceneValidationIndex, _sceneValidationModeOptionNames, _toolbarStyle, guiLayoutOptions);
            EditorGUILayout.EndHorizontal();

            // Output Options
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Output Options", EditorStyles.boldLabel, GUILayout.Width(FIXED_LEFT_COLUMN_WIDTH));
            _outputFormatOptionsIndex = GUILayout.Toolbar(_outputFormatOptionsIndex, _outputFormatOptionNames, _toolbarStyle, guiLayoutOptions);
            AssetValidatorUtility.EditorOuputFormat = _ouputFormatOptions[_outputFormatOptionsIndex];
            EditorGUILayout.EndHorizontal();

            // Debug Options
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Debug Options", EditorStyles.boldLabel, GUILayout.Width(FIXED_LEFT_COLUMN_WIDTH));
            _debugSelectionIndex = GUILayout.Toolbar(_debugSelectionIndex, _debugOptions, _toolbarStyle, guiLayoutOptions);
            AssetValidatorUtility.IsDebugging = _debugSelectionIndex > 0;
            EditorGUILayout.EndHorizontal();


            // Run Validation Button
            // Configuration Header
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Execute", _headerStyle);

            GUI.skin = _originalGUISkin;
            if (GUILayout.Button("Run Validation", _buttonStyle, GUILayout.Height(40f)))
                OnValidateSelectionClick(GetSelectedSceneMode(), DoValidateProjectAssets(), DoValidateAcrossScenes());
            GUI.skin = _customGUISkin;
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
            
            EditorGUILayout.LabelField("Validation Log Views", _headerStyle);
            EditorGUILayout.Separator();

            // Log Grouping Options
            EditorGUI.BeginDisabledGroup(_runner != null && _runner.IsRunning());
            GUI.skin = _originalGUISkin;
            _groupByRect = EditorGUILayout.BeginHorizontal();
            _groupByOptionsIndex = GUILayout.Toolbar(_groupByOptionsIndex, _groupByOptionsNames, _toolbarStyle, guiLayoutOptions);
            EditorGUILayout.EndHorizontal();

            _groupByRect.y += EditorGUIUtility.standardVerticalSpacing * 2f;
            _logRect = new Rect(_groupByRect.xMin, _groupByRect.yMax, _groupByRect.width,
                position.height - _groupByRect.yMin - 25f);

            // Display all logs for the current validation, if any
            GUI.skin = _originalGUISkin;
            EditorGUI.EndDisabledGroup();

            if (!HasLogs() || _runner != null && _runner.IsRunning())
                EditorGUILayout.LabelField("No validation issues found...");
            else if(!IsRunning() && HasLogs())
                OnLogsGUI();
        }

        private void CacheGUIInfo()
        {
            _originalGUISkin = GUI.skin;

            _buttonStyle = _originalGUISkin.GetStyle("button");
            _buttonStyle.alignment = TextAnchor.MiddleCenter;
            _buttonStyle.margin = new RectOffset(10, 10, 10, 0);

            _headerStyle = new GUIStyle(EditorStyles.boldLabel) {alignment = TextAnchor.MiddleCenter};
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
            return _logger != null && _logger.HasLogs();
        }

        private void Update()
        {
            if (_runner == null) return;
            if (!_runner.IsRunning()) return;

            _runner.ContinueRunning();

            ForceEditorWindowUpdate();

            if (!_runner.IsComplete()) return;
            
            if (AssetValidatorUtility.IsDebugging)
                Debug.Log("AssetValidatorRunner has completed...");

            CacheLogs();
            ForceEditorWindowUpdate();

            if (AssetValidatorUtility.EditorOuputFormat != OutputFormat.None)
            {
                using (var writer = new AssetValidatorLogWriter(AssetValidatorUtility.EditorFilename, 
                                                                AssetValidatorUtility.EditorOuputFormat))
                {
                    var logs = _logger.GetLogs();

                    if (AssetValidatorUtility.EditorOuputFormat == OutputFormat.Html)
                        writer.CreateHtmlStyles(logs);

                    writer.AppendHeader();
                    for (var i = 0; i < logs.Count; i++)
                        writer.AppendVLog(logs[i]);

                    writer.AppendFooter();
                    writer.Flush();
                }
            }
        }

        private static float GetEditorTime()
        {
            return (float) EditorApplication.timeSinceStartup;
        }

        private void ForceEditorWindowUpdate()
        {
            Repaint();
        }

        /// <summary>
        /// Launch Validator is a one-all fit for being able to run Validation
        /// </summary>
        /// <param name="vMode"></param>
        /// <param name="oFormat"></param>
        /// <param name="doValidateProjectAssets"></param>
        /// <param name="doValidateAcrossScenes"></param>
        /// <param name="fileName"></param>
        private void LaunchValidator(SceneValidationMode vMode,
                                     OutputFormat oFormat,
                                     bool doValidateProjectAssets = false,
                                     bool doValidateAcrossScenes = false,
                                     string fileName = "asset_validator_results")
        {
            AssetValidatorUtility.EditorOuputFormat = oFormat;
            AssetValidatorUtility.EditorFilename = fileName;

            OnValidateSelectionClick(vMode, doValidateProjectAssets, doValidateAcrossScenes);
        }

        private void OnValidateSelectionClick(SceneValidationMode vmode, bool doValidateProjectAssets = false, bool doValidateAcrossScenes = false)
        {
            // Only perform validation across multiple scenes if the current scene is saved, or 
            // barring that if you don't care if it gets unloaded and lose changes
            var canExecute = true;
            var currentScene = EditorSceneManager.GetActiveScene();
            if (currentScene.isDirty && 
                vmode != SceneValidationMode.None && 
                vmode != SceneValidationMode.ActiveScene)
            {
                var didSave = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() && !currentScene.isDirty;
                if (!didSave)
                    canExecute = EditorUtility.DisplayDialog("Asset Validator",
                                                             "Continuing with validation may unload the current scene. Do you wish to continue?",
                                                             "Continue Validation",
                                                             "Cancel");
            }

            if (!canExecute) return;

            // Clear out any remaining logs and queue up an AssetValidationRunner with 
            // the desired config
            _logger.ClearLogs();
            _runner = new AssetValidatorRunner(_logger, vmode);

            if(doValidateProjectAssets)
                _runner.EnableProjectAssetValidation();

            if(doValidateAcrossScenes)
                _runner.EnableCrossSceneValidation();
        }

#region IDisposable

        public void Dispose()
        {
            if (_runner != null) _runner.Dispose();
        }

#endregion

#if UNITY_5_6_OR_NEWER

        private VLogTreeView _vLogTreeView;

        private void Unsubscribe()
        {
            _vLogTreeView.OnLoadScene -= LoadScenePath;
            _vLogTreeView.OnPingObject -= TryPingObject;

            EditorSceneManager.sceneOpened -= OnSceneLoaded;
        }

        private void InitOnEnable()
        {
            _vLogTreeView = new VLogTreeView(new TreeViewState());
            _vLogTreeView.OnLoadScene += LoadScenePath;
            _vLogTreeView.OnPingObject += TryPingObject;

            EditorSceneManager.sceneOpened += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, OpenSceneMode mode)
        {
            _vLogTreeView.TryReload();
        }

        private void OnLogsGUI()
        {
            _vLogTreeView.SetLogData(_logger.GetLogs());
            _vLogTreeView.SetGroupByMode(_groupByOptionsValues[_groupByOptionsIndex]);
            _vLogTreeView.Reload();
            _vLogTreeView.OnGUI(_logRect);
        }

        // No-Op in 5.6 and later
        private void CacheLogs() { }

#else
        private Vector2 _scrollPos;
        private bool _showProjectAssetsValidationFoldout;
        private bool _showSceneValidationFoldout;
        private bool _showMiscValidationFoldout;
        
        // Logs by Area
        private List<VLog> _projLogs;
        private List<VLog> _sceneLogs;
        private List<VLog> _miscLogs;

        // Logs by Validator
        private ILookup<string, VLog> _logsByValidator;
        private bool[] _foldoutsPerValidator;

        private void InitOnEnable()
        {
            AssetValidatorGraphicsUtility.OnLoadScene += LoadScenePath;
            AssetValidatorGraphicsUtility.OnPingObject += TryPingObject;
        }

        private void Unsubscribe()
        {
            AssetValidatorGraphicsUtility.OnLoadScene += LoadScenePath;
            AssetValidatorGraphicsUtility.OnPingObject += TryPingObject;
        }

        private void OnLogsGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            switch (_groupByOptionsValues[_groupByOptionsIndex])
            {
                case VLogTreeGroupByMode.CollapseByValidatorType:
                    OnLogsByValidatorGUI();
                    break;
                case VLogTreeGroupByMode.CollapseByArea:
                    OnLogsByAreaGUI();
                    break;
            }
            EditorGUILayout.EndScrollView();
        }

        private void OnLogsByValidatorGUI()
        {
            var errorCount = 0;
            var warningCount = 0;
            var infoCount = 0;

            var count = 0;
            foreach (var kvp in _logsByValidator)
            {
                GetCounts(kvp.ToArray(), out errorCount, out warningCount, out infoCount);

                var rect = EditorGUILayout.BeginHorizontal();

                if (Event.current.type == EventType.Repaint)
                {
                    var originalColor = GUI.color;
                    GUI.color = AssetValidatorGraphicsUtility.GetVLogHeaderColor(null);
                    AssetValidatorGraphicsUtility.Styles.HeaderBackground.Draw(rect, false, false, false, false);
                    GUI.color = originalColor;
                }

                _foldoutsPerValidator[count] = EditorGUILayout.Foldout(_foldoutsPerValidator[count], kvp.Key);

                var fieldRect = new Rect(rect)
                {
                    xMin = rect.xMin + rect.width - 150f,
                    width = 50f
                };

                // Now that we have drawn our title, lets draw the aggregate logs if possible
                GUI.Label(fieldRect, new GUIContent(infoCount.ToString(), AssetValidatorGraphicsUtility.InfoIconSmall));
                fieldRect.x += 50f;
                GUI.Label(fieldRect, new GUIContent(warningCount.ToString(), AssetValidatorGraphicsUtility.WarningIconSmall));
                fieldRect.x += 50f;
                GUI.Label(fieldRect, new GUIContent(errorCount.ToString(), AssetValidatorGraphicsUtility.ErrorIconSmall));

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                if (_foldoutsPerValidator[count])
                {
                    foreach (var log in kvp)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(25f);
                        AssetValidatorGraphicsUtility.DrawVLogHeaderGUI(log.validatorName, log);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(25f);
                        AssetValidatorGraphicsUtility.DrawVLogBodyGUI(log);
                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(15f);
                    }
                }

                count++;
            }
        }

        private void GetCounts(IList<VLog> logs, out int errorCount, out int warningCount, out int infoCount)
        {
            errorCount = logs.Count(x => x.vLogType == VLogType.Error);
            warningCount = logs.Count(x => x.vLogType == VLogType.Warning);
            infoCount = logs.Count(x => x.vLogType == VLogType.Info);
        }

        private void OnLogsByAreaGUI()
        {
            var errorCount = 0;
            var warningCount = 0;
            var infoCount = 0;

            GetCounts(_projLogs, out errorCount, out warningCount, out infoCount);

            var projectRect = EditorGUILayout.BeginHorizontal();

            if (Event.current.type == EventType.Repaint)
            {
                var originalColor = GUI.color;
                GUI.color = AssetValidatorGraphicsUtility.GetVLogHeaderColor(null);
                AssetValidatorGraphicsUtility.Styles.HeaderBackground.Draw(projectRect, false, false, false, false);
                GUI.color = originalColor;
            }

            _showProjectAssetsValidationFoldout = EditorGUILayout.Foldout(_showProjectAssetsValidationFoldout, "Project");

            var fieldRect = new Rect(projectRect)
            {
                xMin = projectRect.xMin + projectRect.width - 150f,
                width = 50f
            };

            // Now that we have drawn our title, lets draw the aggregate logs if possible
            GUI.Label(fieldRect, new GUIContent(infoCount.ToString(), AssetValidatorGraphicsUtility.InfoIconSmall));
            fieldRect.x += 50f;
            GUI.Label(fieldRect, new GUIContent(warningCount.ToString(), AssetValidatorGraphicsUtility.WarningIconSmall));
            fieldRect.x += 50f;
            GUI.Label(fieldRect, new GUIContent(errorCount.ToString(), AssetValidatorGraphicsUtility.ErrorIconSmall));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (_showProjectAssetsValidationFoldout)
            {
                EditorGUI.indentLevel++;
                foreach (var projLog in _projLogs)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(25f);
                    AssetValidatorGraphicsUtility.DrawVLogHeaderGUI(projLog.objectPath, projLog);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(25f);
                    AssetValidatorGraphicsUtility.DrawVLogBodyGUI(projLog);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(15f);
                }
                EditorGUI.indentLevel--;
            }

            GetCounts(_sceneLogs, out errorCount, out warningCount, out infoCount);

            var sceneRect = EditorGUILayout.BeginHorizontal();
            if (Event.current.type == EventType.Repaint)
            {
                var originalColor = GUI.color;
                GUI.color = AssetValidatorGraphicsUtility.GetVLogHeaderColor(null);
                AssetValidatorGraphicsUtility.Styles.HeaderBackground.Draw(sceneRect, false, false, false, false);
                GUI.color = originalColor;
            }

            _showSceneValidationFoldout = EditorGUILayout.Foldout(_showSceneValidationFoldout, "Scenes");

            fieldRect = new Rect(sceneRect)
            {
                xMin = sceneRect.xMin + sceneRect.width - 150f,
                width = 50f
            };

            // Now that we have drawn our title, lets draw the aggregate logs if possible
            GUI.Label(fieldRect, new GUIContent(infoCount.ToString(), AssetValidatorGraphicsUtility.InfoIconSmall));
            fieldRect.x += 50f;
            GUI.Label(fieldRect, new GUIContent(warningCount.ToString(), AssetValidatorGraphicsUtility.WarningIconSmall));
            fieldRect.x += 50f;
            GUI.Label(fieldRect, new GUIContent(errorCount.ToString(), AssetValidatorGraphicsUtility.ErrorIconSmall));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (_showSceneValidationFoldout)
            {
                EditorGUI.indentLevel++;
                foreach (var sceneLog in _sceneLogs)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(25f);
                    AssetValidatorGraphicsUtility.DrawVLogHeaderGUI(sceneLog.objectPath, sceneLog);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(25f);
                    AssetValidatorGraphicsUtility.DrawVLogBodyGUI(sceneLog);
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(15f);
                }
                EditorGUI.indentLevel--;
            }

            GetCounts(_miscLogs, out errorCount, out warningCount, out infoCount);

            var miscRect = EditorGUILayout.BeginHorizontal();
            if (Event.current.type == EventType.Repaint)
            {
                var originalColor = GUI.color;
                GUI.color = AssetValidatorGraphicsUtility.GetVLogHeaderColor(null);
                AssetValidatorGraphicsUtility.Styles.HeaderBackground.Draw(miscRect, false, false, false, false);
                GUI.color = originalColor;
            }

            _showMiscValidationFoldout = EditorGUILayout.Foldout(_showMiscValidationFoldout, "Misc");

            fieldRect = new Rect(miscRect)
            {
                xMin = miscRect.xMin + miscRect.width - 140f,
                width = 40f
            };

            // Now that we have drawn our title, lets draw the aggregate logs if possible
            GUI.Label(fieldRect, new GUIContent(infoCount.ToString(), AssetValidatorGraphicsUtility.InfoIconSmall));
            fieldRect.x += 40f;
            GUI.Label(fieldRect, new GUIContent(warningCount.ToString(), AssetValidatorGraphicsUtility.WarningIconSmall));
            fieldRect.x += 40f;
            GUI.Label(fieldRect, new GUIContent(errorCount.ToString(), AssetValidatorGraphicsUtility.ErrorIconSmall));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (_showMiscValidationFoldout)
            {
                EditorGUI.indentLevel++;
                foreach (var miscLog in _miscLogs)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(25f);
                    AssetValidatorGraphicsUtility.DrawVLogHeaderGUI(miscLog.validatorName, miscLog);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(25f);
                    AssetValidatorGraphicsUtility.DrawVLogBodyGUI(miscLog);
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(15f);
                }
                EditorGUI.indentLevel--;
            }
        }

        private void CacheLogs()
        {
            var logs = _logger.GetLogs();
            _projLogs = logs.Where(x => x.source == VLogSource.Project).ToList();
            _projLogs.Sort((x,y) => x.objectPath.CompareTo(y.objectPath));

            _sceneLogs = logs.Where(x => x.source == VLogSource.Scene).ToList();
            _sceneLogs.Sort((x,y) => x.scenePath.CompareTo(y.scenePath));

            _miscLogs = logs.Where(x => x.source == VLogSource.None).ToList();

            _logsByValidator = logs.ToLookup(x => x.validatorName);
            _foldoutsPerValidator = new bool[_logsByValidator.Count];
        }
#endif
    }
}

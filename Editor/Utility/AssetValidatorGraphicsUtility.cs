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
using JCMG.AssetValidator.Editor.Validators.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Utility
{
    /// <summary>
    /// Utility functions for drawing the scene validator window
    /// </summary>
    public static class AssetValidatorGraphicsUtility
    {
        public static Action<VLog> OnLoadScene;
        public static Action<VLog> OnPingObject;

        public const string CrossIconPath = "Assets/JCMG/AssetValidator/Editor/Graphics/cross_icon.png";
        public const string CheckIconPath = "Assets/JCMG/AssetValidator/Editor/Graphics/check_icon.png";
        public const string QuestionIconPath = "Assets/JCMG/AssetValidator/Editor/Graphics/question_icon.png";

        public static GUIStyle ErrorStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.red },
        };

        public static class Styles
        {
            public static GUIStyle BodyBackground = "RL Background";

            private static GUIStyle headerBackground;
            public static GUIStyle HeaderBackground
            {
                get
                {
                    if (headerBackground == null)
                    {
                        headerBackground = new GUIStyle("RL Header")
                        {
                            fixedHeight = 25f
                        };
                    }

                    return headerBackground;
                }
            }

            private static GUIStyle _labelStyle;
            public static GUIStyle LabelStyle
            {
                get
                {
                    return _labelStyle ??
                           (_labelStyle = new GUIStyle(EditorStyles.label)
                           {
                               wordWrap = true
                           });
                }
            }
        }
        private static readonly Color _dgColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        private static readonly Color _gColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        private static Texture2D _gTexture2D;
        public static Texture2D GrayTexture2D
        {
            get { return _gTexture2D ?? (_gTexture2D = CreateUITexture(_gColor)); }
        }

        private static Texture2D CreateUITexture(Color color)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();

            return tex;
        }

        private static Texture2D errorIcon;
        public static Texture2D ErrorIcon
        {
            get
            {
                return errorIcon ?? (errorIcon = EditorGUIUtility.Load("icons/console.erroricon.png") as Texture2D);
            }
        }


        private static Texture2D errorIconSmall;
        public static Texture2D ErrorIconSmall
        {
            get
            {
                return errorIconSmall ?? (errorIconSmall = EditorGUIUtility.Load("icons/console.erroricon.sml.png") as Texture2D);
            }
        }

        private static Texture2D warningIcon;
        public static Texture2D WarningIcon
        {
            get
            {
                return warningIcon ?? (warningIcon = EditorGUIUtility.Load("icons/console.warnicon.png") as Texture2D);
            }
        }

        private static Texture2D warningIconSmall;
        public static Texture2D WarningIconSmall
        {
            get
            {
                return warningIconSmall ?? (warningIconSmall = EditorGUIUtility.Load("icons/console.warnicon.sml.png") as Texture2D);
            }
        }

        private static Texture2D infoIcon;
        public static Texture2D InfoIcon
        {
            get
            {
                return infoIcon ?? (infoIcon = EditorGUIUtility.Load("icons/console.infoicon.png") as Texture2D);
            }
        }

        private static Texture2D infoIconSmall;
        public static Texture2D InfoIconSmall
        {
            get
            {
                return infoIconSmall ?? (infoIconSmall = EditorGUIUtility.Load("icons/console.infoicon.sml.png") as Texture2D);
            }
        }

        public static void DrawVLogHeaderGUI(string label, VLog log)
        {
            var style = GetVLogHeaderStyle(log);
            var rect = EditorGUILayout.BeginHorizontal();

            if (Event.current.type == EventType.Repaint)
            {
                var originalColor = GUI.color;
                GUI.color = GetVLogHeaderColor(log);
                Styles.HeaderBackground.Draw(rect, false, false, false, false);
                GUI.color = originalColor;
            }

            GUILayout.Space(60f);
            GUILayout.Label(label, style);
            EditorGUILayout.EndHorizontal();
        }

        public static GUIStyle GetVLogHeaderStyle(VLog vLog)
        {
            GUIStyle style;
            switch (vLog == null ? VLogType.Info : vLog.vLogType)
            {
                case VLogType.Warning:
                    style = EditorGUIUtility.isProSkin ? EditorStyles.whiteBoldLabel : EditorStyles.boldLabel;
                    break;
                case VLogType.Error:
                    style = EditorGUIUtility.isProSkin ? EditorStyles.boldLabel : EditorStyles.whiteBoldLabel;
                    break;
                case VLogType.Info:
                default:
                    style = EditorGUIUtility.isProSkin ? EditorStyles.boldLabel : EditorStyles.whiteBoldLabel;
                    break;
            }

            return style;
        }

        public static Color GetVLogHeaderColor(VLog log)
        {
            Color color;
            switch (log == null ? VLogType.Info : log.vLogType)
            {
                case VLogType.Warning:
                    color = Color.yellow;
                    break;
                case VLogType.Error:
                    color = Color.red;
                    break;
                case VLogType.Info:
                default:
                    color = _gColor;
                    break;
            }

            return color;
        }

        public static void DrawVLogBodyGUI(VLog log)
        {
            var rect = EditorGUILayout.BeginVertical();

#if UNITY_2017_3_OR_NEWER
            if (Event.current.type == EventType.Repaint)
#else
            if (Event.current.type == EventType.repaint)
#endif
            {
                rect.height += 10f;

                var originalColor = GUI.color;
                GUI.color = _dgColor;
                Styles.BodyBackground.Draw(rect, false, false, false, false);
                GUI.color = originalColor;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Area:   ", EditorStyles.boldLabel, GUILayout.Width(80f));
            EditorGUILayout.LabelField(log.GetSourceDescription(), Styles.LabelStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Object:  ", EditorStyles.boldLabel, GUILayout.Width(80f));
            EditorGUILayout.LabelField(log.objectPath, Styles.LabelStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Message: ", EditorStyles.boldLabel, GUILayout.Width(80f));
            EditorGUILayout.LabelField(log.message, Styles.LabelStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(25f);
            if (log.source == VLogSource.Scene || log.source == VLogSource.Project)
            {
                EditorGUI.BeginDisabledGroup(!log.CanPingObject());
                if (GUILayout.Button("Ping Object", GUILayout.Width(200f)))
                {
                    if (OnPingObject != null)
                        OnPingObject(log);
                }
                EditorGUI.EndDisabledGroup();
            }

            if (log.source == VLogSource.Scene)
            {
                EditorGUI.BeginDisabledGroup(!log.CanLoadScene());
                if (GUILayout.Button("Load Scene", GUILayout.Width(200f)))
                {
                    if (OnLoadScene != null)
                        OnLoadScene(log);
                }
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.Space(25f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        public static string GetLogHeaderInfo(string header, IEnumerable<VLog> logs)
        {
            return string.Format("{0} : {1}", header, GetLogHeaderInfo(logs));
        }

        public static string GetProjectLogHeaderInfo(IEnumerable<VLog> logs)
        {
            return string.Format("Project Assets : {0}", GetLogHeaderInfo(logs));
        }

        public static string GetSceneLogHeaderInfo(IEnumerable<VLog> logs)
        {
            return string.Format("Scenes : {0}", GetLogHeaderInfo(logs));
        }

        public static string GetMiscLogHeaderInfo(IEnumerable<VLog> logs)
        {
            return string.Format("Misc : {0}", GetLogHeaderInfo(logs));
        }

        public static string GetLogHeaderInfo(IEnumerable<VLog> logs)
        {
            var vLogs = logs as VLog[] ?? logs.ToArray();
            return string.Format("{0} Errors, {1} Warnings, {2} Info",
                vLogs.Count(x => x.vLogType == VLogType.Error),
                vLogs.Count(x => x.vLogType == VLogType.Warning),
                vLogs.Count(x => x.vLogType == VLogType.Info));
        }

        private static readonly StringBuilder _multiSB = new StringBuilder();
        public static string GetMultilineString(string message, int charsPerLine)
        {
            if (charsPerLine <= 0)
                charsPerLine = message.Length;

            _multiSB.Remove(0, _multiSB.Length);
            for (var i = 0; i < message.Length; i += charsPerLine)
            {
                if (i + charsPerLine > message.Length) charsPerLine = message.Length - i;
                _multiSB.AppendLine(message.Substring(i, charsPerLine));
            }

            return _multiSB.ToString();
        }
    }
}

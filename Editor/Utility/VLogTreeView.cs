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
#if UNITY_5_6_OR_NEWER

using System;
using System.Collections.Generic;
using JCMG.AssetValidator.Editor.Validators.Output;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Utility
{
    public class VLogTreeView : TreeView
    {
        public Action<VLog> OnLoadScene;
        public Action<VLog> OnPingObject;

        private IList<VLog> _logs;
        private VLogTreeGroupByMode _vLogTreeGroupByMode;
        private float _width;

        public VLogTreeView(TreeViewState state)
            : base(state)
        {
            showAlternatingRowBackgrounds = true;
            showBorder = true;
        }

        public void SetLogData(IList<VLog> logs)
        {
            _logs = logs;
        }

        public void SetGroupByMode(VLogTreeGroupByMode mode)
        {
            _vLogTreeGroupByMode = mode;
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = VLogTreeViewUtility.CreateTreeByGroupType(_vLogTreeGroupByMode, _logs);

            SetupDepthsFromParentsAndChildren(root);

            return root;
        }

        public override void OnGUI(Rect rect)
        {
            _width = rect.width;

            base.OnGUI(rect);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var vLogTreeItem = args.item as VLogTreeViewItem;
            var contentIndent = GetContentIndent(args.item);

            // Background for header and body
            var bgRect = args.rowRect;
            bgRect.x = contentIndent;
            bgRect.width = bgRect.width - contentIndent - 5f;
            bgRect.yMin += 2f;
            bgRect.yMax -= 2f;

            DrawItemBackground(bgRect, vLogTreeItem == null ? null : vLogTreeItem.Log);
            
            // Draw Header Content
            var headerRect = new Rect(bgRect);
            headerRect.xMin += 5f;
            headerRect.height = 25f;
            OnHeaderGUI(headerRect, args.label, args.item);

            // Draw Body Content
            if (vLogTreeItem != null)
            {
                var controlsRect = new Rect(headerRect);
                controlsRect.xMin += 20f;
                controlsRect.y += headerRect.height;

                OnControlsGUI(controlsRect, vLogTreeItem);
            }
        }

        private void OnHeaderGUI(Rect headerRect, string label, TreeViewItem treeViewItem)
        {
            var vLogTreeViewItem = treeViewItem as VLogTreeViewItem;
            var vLogTreeViewHeader = treeViewItem as VLogTreeViewHeader;
            var vLog = vLogTreeViewItem != null ? vLogTreeViewItem.Log : null;
            var style = AssetValidatorGraphicsUtility.GetVLogHeaderStyle(vLog);

            headerRect.y += 1f;

            // Do toggle
            var toggleRect = headerRect;
            toggleRect.width = 16f;

            var labelRect = new Rect(headerRect);
            labelRect.xMin += toggleRect.width + 2f;

            if(vLogTreeViewHeader != null)
            {
                labelRect.width -= 150f;
                
                GUI.Label(labelRect, label, style);

                var fieldRect = new Rect(labelRect)
                {
                    xMin = labelRect.xMin + labelRect.width,
                    width = 50f
                };

                // Now that we have drawn our title, lets draw the aggregate logs if possible
                GUI.Label(fieldRect, new GUIContent(vLogTreeViewHeader.infoCount.ToString(), AssetValidatorGraphicsUtility.InfoIconSmall)); //, EditorStyles.toolbarButton);
                fieldRect.x += 50f;
                GUI.Label(fieldRect, new GUIContent(vLogTreeViewHeader.warningCount.ToString(), AssetValidatorGraphicsUtility.WarningIconSmall)); //, EditorStyles.toolbarButton);
                fieldRect.x += 50f;
                GUI.Label(fieldRect, new GUIContent(vLogTreeViewHeader.errorCount.ToString(), AssetValidatorGraphicsUtility.ErrorIconSmall)); //, EditorStyles.toolbarButton);
            }
            else
            {
                GUI.Label(labelRect, label, style);
            }            
        }

        private void OnControlsGUI(Rect controlsRect, VLogTreeViewItem viewItem)
        {
            var rect = controlsRect;
            rect.y += 3f;
            rect.height = EditorGUIUtility.singleLineHeight;

            var prefixRect = new Rect(rect) { width = 80f };
            var fieldRect = new Rect(rect)
            {
                width = rect.width - prefixRect .width,
                x = rect.x + prefixRect.width
            };

            EditorGUI.LabelField(prefixRect, "Area:   ", EditorStyles.boldLabel);
            EditorGUI.LabelField(fieldRect, viewItem.Log.GetSourceDescription());

            fieldRect.y = prefixRect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            fieldRect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(prefixRect, "Object:  ", EditorStyles.boldLabel);
            EditorGUI.LabelField(fieldRect, viewItem.Log.objectPath);

            fieldRect.y = prefixRect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            fieldRect.height = (rect.height + EditorGUIUtility.standardVerticalSpacing) * 2f;
            
            var charactersPerRow = GetCharactersPerRow(fieldRect, viewItem.Log);
            var multiLineMessage = AssetValidatorGraphicsUtility.GetMultilineString(viewItem.Log.message, charactersPerRow);

            fieldRect.height = 2 * (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(prefixRect, "Message: ", EditorStyles.boldLabel);
            EditorGUI.TextArea(fieldRect, multiLineMessage, EditorStyles.label);

            EditorGUI.LabelField(fieldRect, multiLineMessage);

            rect.y = fieldRect.y + fieldRect.height + EditorGUIUtility.standardVerticalSpacing;
            
            var buttonRect = rect;
            buttonRect.width = 200f;
            buttonRect.height *= 1.25f;

            if ((viewItem.Log.source == VLogSource.Scene || 
                 viewItem.Log.source == VLogSource.Project) && viewItem.Log.HasObjectPath())
            {
                EditorGUI.BeginDisabledGroup(!viewItem.Log.CanPingObject());
                if (GUI.Button(buttonRect, "Ping Object"))
                {
                    if (OnPingObject != null)
                        OnPingObject(viewItem.Log);
                }
                EditorGUI.EndDisabledGroup();
            }

            buttonRect.x += 220f;

            if (viewItem.Log.source == VLogSource.Scene)
            {
                EditorGUI.BeginDisabledGroup(!viewItem.Log.CanLoadScene());
                if (GUI.Button(buttonRect, "Load Scene"))
                {
                    if (OnLoadScene != null)
                        OnLoadScene(viewItem.Log);
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        private void DrawItemBackground(Rect bgRect, VLog log)
        {
#if UNITY_2017_3_OR_NEWER
            if (Event.current.type == EventType.Repaint)
#else
            if (Event.current.type == EventType.repaint)
#endif
            {
                var originalColor = GUI.color;
                GUI.color = AssetValidatorGraphicsUtility.GetVLogHeaderColor(log);
                var rect = bgRect;
                rect.height = 25f;
                AssetValidatorGraphicsUtility.Styles.HeaderBackground.Draw(rect, false, false, false, false);
                GUI.color = originalColor;

                if (log == null) return;

                rect.y += rect.height;
                rect.height = bgRect.height - rect.height;
                AssetValidatorGraphicsUtility.Styles.BodyBackground.Draw(rect, false, false, false, false);
            }
        }

        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            var vLogTreeViewItem = item as VLogTreeViewItem;
            if (vLogTreeViewItem != null)
            {
                if (vLogTreeViewItem.Log.CanLoadScene() || vLogTreeViewItem.Log.CanPingObject())
                    return 145f;
                else
                    return 125f;
            }

            return 30f;
        }

        private int GetNumberOfMessageRows(Rect? rect, VLog item)
        {
            if(!rect.HasValue)
                rect = new Rect(0,0, _width, 0);

            var messageChunkSize = 1f;
            var messageWidth = GUI.skin.label.CalcSize(new GUIContent(item.message));
            if (messageWidth.x > rect.Value.width)
                messageChunkSize = messageWidth.x / rect.Value.width;

            return Mathf.CeilToInt(messageChunkSize);
        }

        private int GetCharactersPerRow(Rect rect, VLog item)
        {
            var messageChunkSize = 1f;
            var messageWidth = GUI.skin.label.CalcSize(new GUIContent(item.message));
            if (messageWidth.x > rect.width)
                messageChunkSize = rect.width / messageWidth.x;

            return Mathf.CeilToInt(messageChunkSize * item.message.Length);
        }

        private static void DrawIcon(Rect position, string iconPath)
        {
            EditorGUI.LabelField(position, new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>(iconPath)));
        }

        public void TryReload()
        {
            if(_logs != null && _logs.Count > 0)
                Reload();
        }
    }
}
#endif
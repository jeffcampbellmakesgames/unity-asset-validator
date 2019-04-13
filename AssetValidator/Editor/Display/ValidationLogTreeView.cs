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
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// A <see cref="TreeView"/> representing one or more groups of <see cref="ValidationLogTreeViewItem"/>s
	/// </summary>
	internal sealed class ValidationLogTreeView : TreeView
	{
		private IReadOnlyList<ValidationLog> _logs;
		private LogGroupingMode _logGroupingMode;

		public ValidationLogTreeView(TreeViewState state)
			: base(state)
		{
			showAlternatingRowBackgrounds = true;
			showBorder = true;
		}

		#region Rendering

		/// <summary>
		/// Creates the full tree of <see cref="ValidationLogTreeView"/> and returns the root element.
		/// </summary>
		/// <returns></returns>
		protected override TreeViewItem BuildRoot()
		{
			var root = ValidationLogTreeViewTools.CreateTreeByGroupType(_logGroupingMode, _logs);

			SetupDepthsFromParentsAndChildren(root);

			return root;
		}

		/// <summary>
		/// Renders the row GUI of an individual <see cref="ValidationLogTreeViewItem"/>
		/// </summary>
		/// <param name="args"></param>
		protected override void RowGUI(RowGUIArgs args)
		{
			var vLogTreeItem = args.item as ValidationLogTreeViewItem;
			var contentIndent = GetContentIndent(args.item);

			// Background for header and body
			var bgRect = args.rowRect;
			bgRect.x = contentIndent;
			bgRect.width = bgRect.width - contentIndent - 5f;
			bgRect.yMin += 2f;
			bgRect.yMax -= 2f;

			DrawRowBackground(bgRect, vLogTreeItem == null ? null : vLogTreeItem.Log);

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

		/// <summary>
		/// Renders the header GUI of an individual <see cref="ValidationLogTreeViewItem"/> or
		/// <see cref="ValidationLogTreeViewHeader"/> based on where we are currently rendering in the
		/// <see cref="ValidationLogTreeView"/>.
		/// </summary>
		/// <param name="headerRect"></param>
		/// <param name="label"></param>
		/// <param name="treeViewItem"></param>
		private static void OnHeaderGUI(Rect headerRect, string label, TreeViewItem treeViewItem)
		{
			var vLogTreeViewItem = treeViewItem as ValidationLogTreeViewItem;
			var vLogTreeViewHeader = treeViewItem as ValidationLogTreeViewHeader;
			var vLog = vLogTreeViewItem != null ? vLogTreeViewItem.Log : null;
			var style = GraphicsTools.GetLogHeaderStyle(vLog);

			headerRect.y += 1f;

			// Do toggle
			var toggleRect = headerRect;
			toggleRect.width = 16f;

			var labelRect = new Rect(headerRect);
			labelRect.xMin += toggleRect.width + 2f;

			// If this header is for a grouping of logs, render the header showing the total count of
			// each type of log present.
			if (vLogTreeViewHeader != null)
			{
				labelRect.width -= 150f;

				GUI.Label(labelRect, label, style);

				var fieldRect = new Rect(labelRect)
				{
					xMin = labelRect.xMin + labelRect.width,
					width = 50f
				};

				// Now that we have drawn our title, lets draw the aggregate logs if possible
				GUI.Label(fieldRect,
					new GUIContent(vLogTreeViewHeader.InfoCount.ToString(),
						GraphicsTools.InfoIconSmall));
				fieldRect.x += 50f;
				GUI.Label(fieldRect,
					new GUIContent(vLogTreeViewHeader.WarningCount.ToString(),
						GraphicsTools.WarningIconSmall));
				fieldRect.x += 50f;
				GUI.Label(fieldRect,
					new GUIContent(vLogTreeViewHeader.ErrorCount.ToString(),
						GraphicsTools.ErrorIconSmall));
			}
			else
			{
				GUI.Label(labelRect, label, style);
			}
		}

		/// <summary>
		/// Renders the controls for an individual <see cref="ValidationLogTreeViewItem"/> <paramref name="viewItem"/>
		/// in <see cref="Rect"/> <paramref name="controlsRect"/>.
		/// </summary>
		/// <param name="controlsRect"></param>
		/// <param name="viewItem"></param>
		private static void OnControlsGUI(Rect controlsRect, ValidationLogTreeViewItem viewItem)
		{
			var rect = controlsRect;
			rect.y += 3f;
			rect.height = EditorGUIUtility.singleLineHeight;

			var prefixRect = new Rect(rect) {width = 80f};
			var fieldRect = new Rect(rect)
			{
				width = rect.width - prefixRect.width,
				x = rect.x + prefixRect.width
			};

			EditorGUI.LabelField(prefixRect, EditorConstants.TreeViewAreaLabel, EditorStyles.boldLabel);
			EditorGUI.LabelField(fieldRect, viewItem.Log.GetSourceDescription());

			prefixRect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
			fieldRect.y = prefixRect.y;
			fieldRect.height = EditorGUIUtility.singleLineHeight;

			EditorGUI.LabelField(prefixRect, EditorConstants.TreeViewObjectLabel, EditorStyles.boldLabel);
			EditorGUI.LabelField(fieldRect, viewItem.Log.objectPath);

			prefixRect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
			fieldRect.y = prefixRect.y;
			fieldRect.height = (rect.height + EditorGUIUtility.standardVerticalSpacing) * 2f;

			var charactersPerRow = GraphicsTools.GetCharactersPerRow(fieldRect, viewItem.Log.message);
			var multiLineMessage = GraphicsTools.GetMultilineString(viewItem.Log.message, charactersPerRow);

			fieldRect.height = 2 * (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight);

			EditorGUI.LabelField(prefixRect, EditorConstants.TreeViewMessageLabel,
				EditorStyles.boldLabel);
			EditorGUI.TextArea(fieldRect, multiLineMessage, EditorStyles.label);

			EditorGUI.LabelField(fieldRect, multiLineMessage);

			rect.y = fieldRect.y + fieldRect.height + EditorGUIUtility.standardVerticalSpacing;

			var buttonRect = rect;
			buttonRect.width = 200f;
			buttonRect.height *= 1.25f;

			if ((viewItem.Log.source == LogSource.Scene ||
			     viewItem.Log.source == LogSource.Project) && viewItem.Log.HasObjectPath())
			{
				EditorGUI.BeginDisabledGroup(!viewItem.Log.CanPingObject());
				if (GUI.Button(buttonRect, EditorConstants.PingObjectButtonText))
				{
					ProjectTools.TryPingObject(viewItem.Log);
				}

				EditorGUI.EndDisabledGroup();
			}

			buttonRect.x += 220f;

			if (viewItem.Log.source == LogSource.Scene)
			{
				EditorGUI.BeginDisabledGroup(!viewItem.Log.CanLoadScene());
				if (GUI.Button(buttonRect, EditorConstants.LoadSceneButtonText))
				{
					EditorSceneManager.OpenScene(viewItem.Log.scenePath);
				}

				EditorGUI.EndDisabledGroup();
			}
		}

		/// <summary>
		/// Draws the appropriate row background for a <see cref="ValidationLog"/> <paramref name="log"/>.
		/// </summary>
		/// <param name="bgRect"></param>
		/// <param name="log"></param>
		private static void DrawRowBackground(Rect bgRect, ValidationLog log)
		{
			if (Event.current.type == EventType.Repaint)
			{
				var originalColor = GUI.color;
				GUI.color = GraphicsTools.GetLogHeaderColor(log);
				var rect = bgRect;
				rect.height = 25f;
				GraphicsTools.LogRowHeaderBackground.Draw(rect, false, false, false, false);
				GUI.color = originalColor;

				if (log == null)
				{
					return;
				}

				rect.y += rect.height;
				rect.height = bgRect.height - rect.height;
				GraphicsTools.LogRowBodyBackground.Draw(rect, false, false, false, false);
			}
		}

		/// <summary>
		/// Returns the height that the row rect should.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		protected override float GetCustomRowHeight(int row, TreeViewItem item)
		{
			var height = 30f;

			var vLogTreeViewItem = item as ValidationLogTreeViewItem;
			if (vLogTreeViewItem != null)
			{
				// If our ValidationLogTreeViewItem will have controls the user can utilize, pad the height
				// otherwise return the default row height.
				if (vLogTreeViewItem.Log.CanLoadScene() || vLogTreeViewItem.Log.CanPingObject())
				{
					height = 145f;
				}
				else
				{
					height = 125f;
				}
			}

			return height;
		}

		#endregion

		#region Helper

		/// <summary>
		/// Sets the collection of <see cref="ValidationLog"/> <paramref name="logs"/> that this
		/// <see cref="ValidationLogTreeView"/> will render.
		/// </summary>
		/// <param name="logs"></param>
		public void SetLogData(IReadOnlyList<ValidationLog> logs)
		{
			_logs = logs;
		}

		/// <summary>
		/// Sets the <see cref="LogGroupingMode"/> <paramref name="mode"/> for how
		/// <see cref="ValidationLog"/>s will be grouped and rendered.
		/// </summary>
		/// <param name="mode"></param>
		public void SetGroupByMode(LogGroupingMode mode)
		{
			_logGroupingMode = mode;
		}

		/// <summary>
		/// Attempts to reload the data and render again.
		/// </summary>
		public void TryReload()
		{
			if (_logs != null && _logs.Count > 0)
			{
				Reload();
			}
		}

		#endregion
	}
}

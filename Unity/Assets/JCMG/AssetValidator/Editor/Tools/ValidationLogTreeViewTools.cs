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
using System.Linq;
using UnityEditor.IMGUI.Controls;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// Helper methods for creating and manipulating <see cref="ValidationLogTreeView"/> instances.
	/// </summary>
	internal static class ValidationLogTreeViewTools
	{
		/// <summary>
		/// Sorts <see cref="ValidationLog"/>s based on <seealso cref="ValidationLog.scenePath" /> values.
		/// </summary>
		private class SortValidationLogOnScenePath : IComparer<ValidationLog>
		{
			public int Compare(ValidationLog x, ValidationLog y)
			{
				return string.Compare(x.scenePath, y.scenePath, StringComparison.Ordinal);
			}
		}

		/// <summary>
		/// Sorts <see cref="ValidationLog"/>s based on <seealso cref="ValidationLog.objectPath" /> values.
		/// </summary>
		private class SortValidationLogOnObjectPath : IComparer<ValidationLog>
		{
			public int Compare(ValidationLog x, ValidationLog y)
			{
				return string.Compare(x.objectPath, y.objectPath, StringComparison.Ordinal);
			}
		}

		private const string SceneArea = "Scene";
		private const string ProjectArea = "Project";
		private const string NoneArea = "Misc";

		/// <summary>
		/// Returns a root element <see cref="TreeViewItem"/> for a <see cref="ValidationLogTreeView"/> where
		/// the grouping mode is determined by <see cref="LogGroupingMode"/> <paramref name="mode"/> and the
		/// values are based on a collection of <see cref="ValidationLog"/>(s) in <paramref name="logs"/>.
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="logs"></param>
		/// <returns></returns>
		public static TreeViewItem CreateTreeByGroupType(LogGroupingMode mode, IReadOnlyList<ValidationLog> logs)
		{
			switch (mode)
			{
				case LogGroupingMode.CollapseByValidatorType:
					return CreateTreeGroupedByValidatorType(logs);

				case LogGroupingMode.CollapseByArea:
					return CreateTreeGroupedByArea(logs);

				default:
					throw new ArgumentOutOfRangeException(Enum.GetName(typeof(LogGroupingMode), mode));
			}
		}

		/// <summary>
		/// Returns a root element <see cref="TreeViewItem"/> for a <see cref="ValidationLogTreeView"/> where
		/// the values are based on a collection of <see cref="ValidationLog"/>(s) in <paramref name="logs"/>
		/// and grouped by validator type.
		/// </summary>
		/// <param name="logs"></param>
		/// <returns></returns>
		private static TreeViewItem CreateTreeGroupedByValidatorType(IReadOnlyList<ValidationLog> logs)
		{
			var id = -1;
			var rootItem = new TreeViewItem(++id, -1);

			// Create a lookup of all vLogs grouped by validator name
			var dict = new Dictionary<string, List<ValidationLog>>();
			foreach (var vLog in logs)
			{
				if (!dict.ContainsKey(vLog.validatorName))
				{
					dict[vLog.validatorName] = new List<ValidationLog> {vLog};
				}
				else
				{
					dict[vLog.validatorName].Add(vLog);
				}
			}

			// From the lookup and root item, create a child object for each validator
			// type and for each validator type add all logs of that type as children.
			foreach (var kvp in dict)
			{
				var header = new ValidationLogTreeViewHeader(++id, 0, kvp.Key);
				var kLogs = kvp.Value;
				foreach (var kLog in kLogs)
				{
					header.AddChild(new ValidationLogTreeViewItem(kLog, ++id, 1));
				}

				SetLogCounts(header, kLogs);

				rootItem.AddChild(header);
			}

			return rootItem;
		}

		/// <summary>
		/// Returns a root element <see cref="TreeViewItem"/> for a <see cref="ValidationLogTreeView"/> where
		/// the values are based on a collection of <see cref="ValidationLog"/>(s) in <paramref name="logs"/>
		/// and grouped by <see cref="LogSource"/>.
		/// </summary>
		/// <param name="logs"></param>
		/// <returns></returns>
		private static TreeViewItem CreateTreeGroupedByArea(IReadOnlyList<ValidationLog> logs)
		{
			var id = -1;
			var rootItem = new TreeViewItem(++id, -1);
			var sceneLogs = logs.Where(x => x.source == LogSource.Scene);
			var rootSceneItem = new ValidationLogTreeViewHeader(++id, 0, SceneArea);
			SetLogCounts(rootSceneItem, sceneLogs);

			var projLogs = logs.Where(x => x.source == LogSource.Project);
			var projRootItem = new ValidationLogTreeViewHeader(++id, 0, ProjectArea);
			SetLogCounts(projRootItem, projLogs);

			var miscLogs = logs.Where(x => x.source == LogSource.None);
			var miscRoot = new ValidationLogTreeViewHeader(++id, 0, NoneArea);
			SetLogCounts(miscRoot, miscLogs);

			// Create a lookup of all vLogs grouped by validator name
			var dict = new Dictionary<LogSource, List<ValidationLog>>();
			foreach (var vLog in logs)
			{
				if (!dict.ContainsKey(vLog.source))
				{
					dict[vLog.source] = new List<ValidationLog> {vLog};
				}
				else
				{
					dict[vLog.source].Add(vLog);
				}
			}

			// From the lookup and root item, create a child object for each validator
			// type and for each validator type add all logs of that type as children.
			foreach (var kvp in dict)
			{
				var kLogs = kvp.Value;

				switch (kvp.Key)
				{
					case LogSource.None:
						foreach (var kLog in kLogs)
						{
							miscRoot.AddChild(new ValidationLogTreeViewItem(kLog, ++id, 1, kLog.validatorName));
						}
						break;

					case LogSource.Scene:
						var sceneDict = new Dictionary<string, IList<ValidationLog>>();
						kLogs.Sort(new SortValidationLogOnScenePath());
						foreach (var kLog in kLogs)
						{
							if (sceneDict.ContainsKey(kLog.scenePath))
							{
								sceneDict[kLog.scenePath].Add(kLog);
							}
							else
							{
								sceneDict[kLog.scenePath] = new List<ValidationLog> {kLog};
							}
						}

						foreach (var sceneToLogsKvp in sceneDict)
						{
							var slogs = sceneToLogsKvp.Value;
							var sceneRootItem = new ValidationLogTreeViewHeader(
								++id,
								1,
								sceneToLogsKvp.Key.Split(EditorConstants.ForwardSlashChar).Last());

							foreach (var slog in slogs)
							{
								sceneRootItem.AddChild(new ValidationLogTreeViewItem(
									slog,
									++id,
									2,
									slog.objectPath.Split(EditorConstants.ForwardSlashChar).Last()));

							}

							SetLogCounts(sceneRootItem, slogs);

							rootSceneItem.AddChild(sceneRootItem);
						}

						break;

					case LogSource.Project:
						kLogs.Sort(new SortValidationLogOnObjectPath());
						foreach (var kLog in kLogs)
						{
							projRootItem.AddChild(new ValidationLogTreeViewItem(
								kLog,
								++id,
								1,
								kLog.objectPath.Split(EditorConstants.ForwardSlashChar).Last()));
						}

						break;
				}
			}

			rootItem.AddChild(projRootItem);
			rootItem.AddChild(rootSceneItem);
			rootItem.AddChild(miscRoot);

			return rootItem;
		}

		/// <summary>
		/// Sets appropriate counts per <see cref="LogType"/> on the passed <see cref="ValidationLogTreeViewHeader"/>
		/// <paramref name="header"/> based on the <see cref="ValidationLog"/>(s) in <paramref name="logs"/>.
		/// </summary>
		/// <param name="header"></param>
		/// <param name="logs"></param>
		private static void SetLogCounts(ValidationLogTreeViewHeader header, IEnumerable<ValidationLog> logs)
		{
			var errorCount = 0;
			var warnCount = 0;
			var infoCount = 0;

			foreach (var x in logs)
			{
				switch (x.logType)
				{
					case LogType.Error:
						errorCount++;
						break;
					case LogType.Warning:
						warnCount++;
						break;
					case LogType.Info:
						infoCount++;
						break;
					default:
						throw new ArgumentOutOfRangeException(Enum.GetName(typeof(LogType), x.logType));
				}
			}

			header.SetLogCounts(errorCount, warnCount, infoCount);
		}
	}
}

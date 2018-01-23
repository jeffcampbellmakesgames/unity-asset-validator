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
using System.Linq;
using JCMG.AssetValidator.Editor.Validators.Output;
using JCMG.AssetValidator.Editor.Window;
using UnityEditor.IMGUI.Controls;

namespace JCMG.AssetValidator.Editor.Utility
{
    public static class VLogTreeViewUtility
    {
        public static TreeViewItem CreateTreeByGroupType(VLogTreeGroupByMode mode, IList<VLog> logs)
        {
            switch (mode)
            {
                case VLogTreeGroupByMode.CollapseByValidatorType:
                    return CreateTreeGroupedByValidatorType(logs);
                case VLogTreeGroupByMode.CollapseByArea:
                    return CreateTreeGroupedByArea(logs);
                default:
                    throw new ArgumentOutOfRangeException("mode", mode, null);
            }
        }

        public static TreeViewItem CreateTreeGroupedByValidatorType(IList<VLog> logs)
        {
            var id = -1;
            var rootItem = new TreeViewItem(++id, -1);

            // Create a lookup of all vLogs grouped by validator name
            var dict = new Dictionary<string, List<VLog>>();
            foreach (var vLog in logs)
            {
                if (!dict.ContainsKey(vLog.validatorName))
                    dict[vLog.validatorName] = new List<VLog> { vLog };
                else
                    dict[vLog.validatorName].Add(vLog);
            }

            // From the lookup and root item, create a child object for each validator
            // type and for each validator type add all logs of that type as children.
            foreach (var kvp in dict)
            {
                var header = new VLogTreeViewHeader(++id, 0, kvp.Key);
                var kLogs = kvp.Value;
                foreach (var kLog in kLogs)
                    header.AddChild(new VLogTreeViewItem(kLog, ++id, 1));

                SetLogCounts(header, kLogs);
                
                rootItem.AddChild(header);
            }

            return rootItem;
        }

        private static void SetLogCounts(VLogTreeViewHeader header, IEnumerable<VLog> logs)
        {
            var errorCount = logs.Count(x => x.vLogType == VLogType.Error);
            var warnCount = logs.Count(x => x.vLogType == VLogType.Warning);
            var infoCount = logs.Count(x => x.vLogType == VLogType.Info);

            header.SetLogCounts(errorCount, warnCount, infoCount);
        }

        public static TreeViewItem CreateTreeGroupedByArea(IList<VLog> logs)
        {
            var id = -1;
            var rootItem = new TreeViewItem(++id, -1);
            var sceneLogs = logs.Where(x => x.source == VLogSource.Scene);
            var rootSceneItem = new VLogTreeViewHeader(++id, 0, "Scene");
            SetLogCounts(rootSceneItem, sceneLogs);

            var projLogs = logs.Where(x => x.source == VLogSource.Project);
            var projRootItem = new VLogTreeViewHeader(++id, 0, "Project");
            SetLogCounts(projRootItem, projLogs);

            var miscLogs = logs.Where(x => x.source == VLogSource.None);
            var miscRoot = new VLogTreeViewHeader(++id, 0, "Misc");
            SetLogCounts(miscRoot, miscLogs);

            // Create a lookup of all vLogs grouped by validator name
            var dict = new Dictionary<VLogSource, List<VLog>>();
            foreach (var vLog in logs)
            {
                if (!dict.ContainsKey(vLog.source))
                    dict[vLog.source] = new List<VLog> { vLog };
                else
                    dict[vLog.source].Add(vLog);
            }

            // From the lookup and root item, create a child object for each validator
            // type and for each validator type add all logs of that type as children.
            foreach (var kvp in dict)
            {
                var kLogs = kvp.Value;

                switch (kvp.Key)
                {
                    case VLogSource.None:
                        foreach (var kLog in kLogs)
                            miscRoot.AddChild(new VLogTreeViewItem(kLog, ++id, 1, kLog.validatorName));
                        break;
                    case VLogSource.Scene:
                        var sceneDict = new Dictionary<string, IList<VLog>>();
                        kLogs.Sort(new SortVLogOnScenePath());
                        foreach (var kLog in kLogs)
                        {
                            if (sceneDict.ContainsKey(kLog.scenePath))
                                sceneDict[kLog.scenePath].Add(kLog);
                            else
                                sceneDict[kLog.scenePath] = new List<VLog> {kLog};
                        }

                        foreach (var skvp in sceneDict)
                        {
                            var slogs = skvp.Value;
                            var sceneRootItem = new VLogTreeViewHeader(++id, 1, skvp.Key.Split('/').Last());
                            foreach (var slog in slogs)
                                sceneRootItem.AddChild(new VLogTreeViewItem(slog, ++id, 2, slog.objectPath.Split('/').Last()));

                            SetLogCounts(sceneRootItem, slogs);

                            rootSceneItem.AddChild(sceneRootItem);
                        }
                        
                        break;
                    case VLogSource.Project:
                        kLogs.Sort(new SortVLogOnObjectPath());
                        foreach (var kLog in kLogs)
                            projRootItem.AddChild(new VLogTreeViewItem(kLog, ++id, 1, kLog.objectPath.Split('/').Last()));

                        break;
                }
            }

            rootItem.AddChild(projRootItem);
            rootItem.AddChild(rootSceneItem);
            rootItem.AddChild(miscRoot);

            return rootItem;
        }

        private class SortVLogOnScenePath : IComparer<VLog>
        {
            public int Compare(VLog x, VLog y)
            {
                return x.scenePath.CompareTo(y.scenePath);
            }
        }

        private class SortVLogOnObjectPath : IComparer<VLog>
        {
            public int Compare(VLog x, VLog y)
            {
                return x.objectPath.CompareTo(y.objectPath);
            }
        }
    }
}
#endif
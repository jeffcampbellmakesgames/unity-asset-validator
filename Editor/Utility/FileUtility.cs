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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Utility
{
    public static class FileUtility
    {
        public static void ReduceAssetPathsToFileNames(List<string> bundleNames)
        {
            for (int i = 0; i < bundleNames.Count; i++)
                bundleNames[i] = bundleNames[i].Split('/').Last();
        }

        /// <summary>
        /// Returns AssetDatabase compatible file paths (relative to the Asset folder) for all files 
        /// that have a matching extension in extensions
        /// </summary>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static List<string> GetUnityFilePaths(string[] extensions)
        {
            var list = new List<string>();
            for (var i = 0; i < extensions.Length; i++)
            {
                extensions[i] = extensions[i].Replace(".", string.Empty);

                var filePaths = Directory.GetFiles(Application.dataPath, string.Format("*.{0}", extensions[i]), SearchOption.AllDirectories);

                for (var j = 0; j < filePaths.Length; j++)
                    list.Add(GetUnityRelativePath(filePaths[j]));
            }

            return list;
        }

        /// <summary>
        /// Returns AssetDatabase compatible file paths (relative to the Asset folder) for all files 
        /// that have a matching extension to extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static List<string> GetUnityFilePaths(string extension)
        {
            var list = new List<string>();
            extension = extension.Replace(".", string.Empty);

            var filePaths = Directory.GetFiles(Application.dataPath, string.Format("*.{0}", extension), SearchOption.AllDirectories);

            for (var j = 0; j < filePaths.Length; j++)
                list.Add(GetUnityRelativePath(filePaths[j]));

            return list;
        }

        private static readonly StringBuilder AssetStringBuilder = new StringBuilder();
        private static string GetUnityRelativePath(string absoluteFilePath)
        {
            AssetStringBuilder.Remove(0, AssetStringBuilder.Length);

            absoluteFilePath = absoluteFilePath.Replace("\\", "/");
            var splitFileName = absoluteFilePath.Split('/');

            var assetIndex = FindIndex(splitFileName, "Assets");

            for (var i = assetIndex; i < splitFileName.Length; i++)
            {
                if (i == assetIndex)
                    AssetStringBuilder.Append(splitFileName[i]);
                else
                    AssetStringBuilder.Append(string.Format("/{0}", splitFileName[i]));
            }

            return AssetStringBuilder.ToString();
        }

        private static int FindIndex(IList<string> strArray, string value)
        {
            for (var i = 0; i < strArray.Count; i++)
                if (strArray[i] == value)
                    return i;

            return strArray.Count;
        }
    }
}

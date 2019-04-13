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
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="ValidationLogTreeViewHeader"/> represents a group of related <see cref="ValidationLogTreeViewItem"/>
	/// entries and indicates the counts of informational, warning, and error logs present.
	/// </summary>
	internal sealed class ValidationLogTreeViewHeader : TreeViewItem
	{
		/// <summary>
		/// The number of errors grouped underneath this <see cref="ValidationLogTreeViewHeader"/>.
		/// </summary>
		public int ErrorCount { get; private set; }

		/// <summary>
		/// The number of warnings grouped underneath this <see cref="ValidationLogTreeViewHeader"/>.
		/// </summary>
		public int WarningCount { get; private set; }

		/// <summary>
		/// The number of info messages grouped underneath this <see cref="ValidationLogTreeViewHeader"/>.
		/// </summary>
		public int InfoCount { get; private set; }

		/// <summary>
		/// Empty-constructor to allow access to the base <see cref="TreeViewItem"/> constructor equivalent.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="depth"></param>
		/// <param name="displayName"></param>
		public ValidationLogTreeViewHeader(int id, int depth, string displayName)
			: base(id, depth, displayName)
		{
		}

		/// <summary>
		/// Sets the number of error, warning, and informational log entries visible on this header.
		/// </summary>
		/// <param name="errors"></param>
		/// <param name="warnings"></param>
		/// <param name="infos"></param>
		public void SetLogCounts(int errors, int warnings, int infos)
		{
			ErrorCount = Mathf.Clamp(errors, 0, 999);
			WarningCount = Mathf.Clamp(warnings, 0, 999);
			InfoCount = Mathf.Clamp(infos, 0, 999);
		}
	}
}

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

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="ValidationLogTreeViewItem"/> representing a single <see cref="ValidationLog"/>
	/// </summary>
	internal sealed class ValidationLogTreeViewItem : TreeViewItem
	{
		/// <summary>
		/// The <see cref="ValidationLog"/> associated with this <see cref="ValidationLogTreeViewItem"/>.
		/// </summary>
		public ValidationLog Log { get; }

		/// <summary>
		/// Constructor that accepts a <see cref="ValidationLog"/> and sets an empty display name.
		/// </summary>
		/// <param name="log"></param>
		/// <param name="id"></param>
		/// <param name="depth"></param>
		public ValidationLogTreeViewItem(ValidationLog log, int id, int depth)
			: base(id, depth, string.Empty)
		{
			Log = log;
		}

		/// <summary>
		/// Constructor that accepts a <see cref="ValidationLog"/> and a custom <see cref="string"/>
		/// <paramref name="displayName"/>.
		/// </summary>
		/// <param name="log"></param>
		/// <param name="id"></param>
		/// <param name="depth"></param>
		/// <param name="displayName"></param>
		public ValidationLogTreeViewItem(ValidationLog log, int id, int depth, string displayName)
			: base(id, depth, displayName)
		{
			Log = log;
		}
	}
}

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

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// A <see cref="ResourcePathContract"/> defines a set of assets by resource path that should be available
	/// for loading using the static <see cref="UnityEngine.Resources"/> class. These contracts will be found
	/// via reflection and validated in the editor to make sure that each string path returned from
	/// <see cref="GetPaths"/> resolves to an asset.
	/// </summary>
	public abstract class ResourcePathContract
	{
		/// <summary>
		/// Returns an <see cref="IEnumerable{T}"/> of <see cref="string"/> where each should resolve to an
		/// asset loaded by using <seealso cref="UnityEngine.Resources.Load(string)"/>
		/// </summary>
		/// <returns></returns>
		public abstract IEnumerable<string> GetPaths();
	}
}

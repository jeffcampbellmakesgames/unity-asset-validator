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

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="LogSource"/> indicates the general location of the <see cref="UnityEngine.Object"/>
	/// instance, if any, the log is associated with.
	/// </summary>
	[Serializable]
	public enum LogSource
	{
		/// <summary>
		/// The log is not associated with a specific area in the Unity Editor.
		/// </summary>
		None,

		/// <summary>
		/// The log is associated with a specific scene.
		/// </summary>
		Scene,

		/// <summary>
		/// The log is associated with the Project's Assets folder in the Unity Editor.
		/// </summary>
		Project
	}
}

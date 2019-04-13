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
using System.Runtime.Serialization;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="FileOutputFormat"/> indicates the mode in which validation logs will be written to a file.
	/// </summary>
	[Serializable]
	public enum FileOutputFormat
	{
		/// <summary>
		/// Logs will not be written to a file
		/// </summary>
		[EnumMember(Value = "No Output")]
		None = 0,

		/// <summary>
		/// Logs will be written to a HTML file.
		/// </summary>
		[EnumMember(Value = "Html")]
		Html = 1,

		/// <summary>
		/// Logs will be written to a CSV file.
		/// </summary>
		[EnumMember(Value = "Csv")]
		Csv = 2,

		/// <summary>
		/// Logs will be written to a plaintext file.
		/// </summary>
		[EnumMember(Value = "Text")]
		Text = 3
	}
}

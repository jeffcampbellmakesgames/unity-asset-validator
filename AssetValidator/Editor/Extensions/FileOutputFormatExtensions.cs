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
namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// Helper extensions for <see cref="FileOutputFormat"/>.
	/// </summary>
	internal static class FileOutputFormatExtensions
	{
		private const string HtmlExtension = ".html";
		private const string CsvExtension = ".csv";
		private const string TextExtension = ".txt";

		public static string GetOutputFormatExtension(this FileOutputFormat format)
		{
			switch (format)
			{
				case FileOutputFormat.Html:
					return HtmlExtension;

				case FileOutputFormat.Csv:
					return CsvExtension;

				case FileOutputFormat.Text:
					return TextExtension;

				case FileOutputFormat.None:
				default:
					return string.Empty;
			}
		}
	}
}

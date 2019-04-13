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
using System.IO;
using System.Linq;
using System.Text;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="LogFileWriter"/> is a utility class for writing a collection of
	/// <see cref="ValidationLog"/>(s) to a file.
	/// </summary>
	internal static class LogFileWriter
	{
		private static readonly StringBuilder _stringBuilder = new StringBuilder();

		#region HTML
		private static readonly StringBuilder _styleStringBuilder = new StringBuilder();
		private static readonly Dictionary<string, string> _validatorToHtmlStyle =
			new Dictionary<string, string>();

		private const string HtmlScript = @"
        <script>
	        function hide_class(button, name, className)
	        {
	          elements = getElementsByClass(className);
	          pattern = new RegExp(""(^|\\s)Button(\\s|$)"");

              if(button.value.indexOf(""Hide"") == -1)
              {
                 button.value = ""Hide "" + name;
              }
              else
              {
                 button.value = ""Show "" + name;
              }

                for(i = 0; i<elements.length; i++)
                {
                    console.log(elements[i])

                    if(!pattern.test(elements[i].className))
                    {
                        if(elements[i].style.display != 'none')
                        {
                            elements[i].style.display = 'none'
                        }
                        else
                        {
                            elements[i].style.display = ''
                        }
                    }
                }
            }

            function getElementsByClass(searchClass, node, tag)
            {
            var classElements = new Array();

                if (node == null)
            {
                node = document;
            }
            if (tag == null)
            {
                tag = '*';
            }

            var element = node.getElementsByTagName(tag);
            var elementLength = element.length;
            var pattern = new RegExp(""(^|\\s)"" + searchClass + ""(\\s|$)"");

                for (i = 0, j = 0; i < elementLength; i++)
            {
                if (pattern.test(element[i].className))
                {
                    classElements[j] = element[i];
                    j++;
                }
            }

            return classElements;
        }
        </script>
        ";

		private const string HtmlDocAndHeaderStart = @"
		<!DOCTYPE html>
		<html>
		<head>
		<style>
		p {
	    font-family:""Calibr"";
	    margin:0;
	    }

	    table {
	    width: 100 %;
	    }

	    table, th, td {
	    border: 1px solid black;
	    border - collapse:
	    collapse;
	    }

	    th, td {
	    padding: 5px;
	    text - align: left;
	    }

	    .btn-group {
	        display: flex; /* 2. display flex to the rescue */
	        flex-direction: row;
	    }

	    .btn-group label, input {
	      display: block; /* 1. oh noes, my inputs are styled as block... */
	    }
		";

		private const string HtmlHeaderInnerFormat = "{0}</style>";

		private const string HtmlHeaderEndFormat = @"
					<title>Asset Validation Results: {0}</title>
					</head>
					<body>
					<table>";

		private const string HtmlDocEnd = @"
		</table>
		</body>
		</html>
		";

		private const string HtmlDivStart = "<div class=\"\".btn-group\"\">";
		private const string HtmlDivInnerFormat =
			"<input type=\"button\" value=\"Hide {0}\" class=\"system button\" onclick=\"hide_class(this, '{0}', '{1}')\" />";
		private const string HtmlDivEnd = "</div>";

		private const string HtmlTableStartAndHeader =
			"<tr><th>Validator</th><th>logType</th><th>Source</th><th>Message</th><th>ScenePath</th><th>ObjectPath</th></tr>";
		private const string HtmlTableRowFormat =
			"<tr {0}><th>{1}</th><th>{2}</th><th>{3}</th><th>{4}</th><th>{5}</th><th>{6}</th></tr>";

		private const string CssValidatorStartFormat = "class=\"container {0}\"";
		private const string CssValidatorInnerAndEndFormat = ".{0} {{ }}";

		#endregion

		#region CSV

		private const string CsvHeader = "Validator,logType,Source,Message,ScenePath,ObjectPath";
		private const string CsvRowFormat = "{0},{1},{2},{3},{4},{5}";

		#endregion

		#region PlainText

		private const string PlainTextHeader = "Validator    logType    Source    Message    ScenePath    ObjectPath";
		private const string PlainTextRowFormat = "{0}    {1}    {2}    {3}    {4}    {5}";
		private const string PlainTextFooter = "AssetValidator Complete!";

		#endregion

		/// <summary>
		/// Writes a read-only collection of <see cref="ValidationLog"/>(s) to a file at
		/// <paramref name="filePath"/> in the specified <see cref="FileOutputFormat"/>
		/// <paramref name="outputFormat"/>. The file extension is automatically added based on the
		/// <paramref name="outputFormat"/>.
		///
		/// Where <paramref name="outputFormat"/> is equal to <seealso cref="FileOutputFormat.None"/>, a
		/// log file will not be written.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="outputFormat"></param>
		/// <param name="logs"></param>
		public static void WriteLogs(
			string filePath,
			FileOutputFormat outputFormat,
			IReadOnlyList<ValidationLog> logs)
		{
			// If our selected output format is none, no log file writing needed.
			if (outputFormat == FileOutputFormat.None)
			{
				return;
			}

			AppendHeader(logs, outputFormat);
			foreach (var log in logs)
			{
				AppendLog(log, outputFormat);
			}

			AppendFooter(outputFormat);
			Flush(filePath + outputFormat.GetOutputFormatExtension());
		}

		private static void AppendHeader(IReadOnlyList<ValidationLog> logs, FileOutputFormat outputFormat)
		{
			switch (outputFormat)
			{
				case FileOutputFormat.Html:
					_stringBuilder.Append(HtmlDocAndHeaderStart);
					_stringBuilder.AppendLine(string.Format(HtmlHeaderInnerFormat, CreateHtmlStyles(logs)));
					_stringBuilder.AppendLine(string.Format(HtmlHeaderEndFormat, DateTime.Now));
					_stringBuilder.AppendLine(HtmlScript);

					foreach (var kvp in _validatorToHtmlStyle)
					{
						_stringBuilder.AppendLine(HtmlDivStart);
						_stringBuilder.Append(string.Format(HtmlDivInnerFormat,kvp.Key, kvp.Key.ToLowerInvariant()));
						_stringBuilder.Append(HtmlDivEnd);
					}

					_stringBuilder.AppendLine(HtmlTableStartAndHeader);
					break;

				case FileOutputFormat.Csv:
					_stringBuilder.AppendLine(CsvHeader);
					break;

				case FileOutputFormat.Text:
					_stringBuilder.AppendLine(PlainTextHeader);
					break;

				default:
					var msg = string.Format(EditorConstants.OutputFormatIsInvalid, outputFormat);
					throw new ArgumentOutOfRangeException(msg);
			}
		}

		private static string CreateHtmlStyles(IReadOnlyList<ValidationLog> logs)
		{
			_validatorToHtmlStyle.Clear();
			_styleStringBuilder.Clear();

			var uniqueValidators = logs.Select(x => x.validatorName.ToLowerInvariant()).Distinct();
			foreach (var uniqueValidator in uniqueValidators)
			{
				_validatorToHtmlStyle.Add(uniqueValidator, string.Format(CssValidatorStartFormat, uniqueValidator));

				_styleStringBuilder.AppendLine(string.Format(CssValidatorInnerAndEndFormat, uniqueValidator));
			}

			return _styleStringBuilder.ToString();
		}

		private static void AppendLog(ValidationLog validationLog, FileOutputFormat outputFormat)
		{
			switch (outputFormat)
			{
				case FileOutputFormat.Html:
					string classStyle;
					if (!_validatorToHtmlStyle.TryGetValue(validationLog.validatorName, out classStyle))
					{
						classStyle = string.Empty;
					}

					_stringBuilder.AppendLine(string.Format(HtmlTableRowFormat,
						classStyle,
						validationLog.validatorName,
						validationLog.logType,
						validationLog.source,
						validationLog.message,
						validationLog.scenePath,
						validationLog.objectPath));
					break;

				case FileOutputFormat.Csv:
					_stringBuilder.AppendLine(string.Format(CsvRowFormat,
						validationLog.validatorName,
						validationLog.logType,
						validationLog.source,
						validationLog.message,
						validationLog.scenePath,
						validationLog.objectPath));
					break;

				case FileOutputFormat.Text:
					_stringBuilder.AppendLine(string.Format(PlainTextRowFormat,
						validationLog.validatorName,
						validationLog.logType,
						validationLog.source,
						validationLog.message,
						validationLog.scenePath,
						validationLog.objectPath));
					break;

				default:
					var msg = string.Format(EditorConstants.OutputFormatIsInvalid, outputFormat);
					throw new ArgumentOutOfRangeException(msg);
			}
		}

		private static void AppendFooter(FileOutputFormat outputFormat)
		{
			switch (outputFormat)
			{
				case FileOutputFormat.Html:
					_stringBuilder.Append(HtmlDocEnd);
					break;

				case FileOutputFormat.Csv:
					// Do nothing
					break;

				case FileOutputFormat.Text:
					_stringBuilder.AppendLine(PlainTextFooter);
					break;

				default:
					var msg = string.Format(EditorConstants.OutputFormatIsInvalid, outputFormat);
					throw new ArgumentOutOfRangeException(msg);
			}
		}

		private static void Flush(string filePath)
		{
			if (_stringBuilder.Length == 0)
			{
				return;
			}

			File.WriteAllText(filePath, _stringBuilder.ToString());
		}
	}
}

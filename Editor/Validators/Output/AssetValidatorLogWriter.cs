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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JCMG.AssetValidator.Editor.Utility;

namespace JCMG.AssetValidator.Editor.Validators.Output
{
    public sealed class AssetValidatorLogWriter : IDisposable
    {
        private readonly string _fileName;
        private readonly OutputFormat _oFormat;
        private readonly StringBuilder _stringBuilder;

        // HTML specific styling field
        private string _validatorStyle;
        private Dictionary<string, string> _validatorToHtmlStyle;

        public AssetValidatorLogWriter(string fileName, OutputFormat oFormat)
        {
            _fileName = fileName + OutputFormatUtility.GetOutputFormatExtension(oFormat);
            _oFormat = oFormat;

            _stringBuilder = new StringBuilder();
        }

        public void CreateHtmlStyles(IList<VLog> logs)
        {
            _validatorToHtmlStyle = new Dictionary<string, string>();

            var styleBuilder = new StringBuilder();
            var uniqueValidators = logs.Select(x => x.validatorName).Distinct().ToList();
            uniqueValidators.Sort();
            foreach (var uniqueValidator in uniqueValidators)
            {
                var className = uniqueValidator.ToLower();

                _validatorToHtmlStyle.Add(uniqueValidator, string.Format("class=\"container {0}\"", className));

                styleBuilder.AppendLine(string.Format(@".{0} {{ }}", className));
            }

            _validatorStyle = styleBuilder.ToString();
        }

        public void AppendHeader()
        {
            switch (_oFormat)
            {
                case OutputFormat.Html:
                    // Header
                    _stringBuilder.AppendLine("<!DOCTYPE html>");
                    _stringBuilder.AppendLine("<html>");
                    _stringBuilder.AppendLine("<head>");
                    _stringBuilder.AppendLine("<style>\n" + _style + _validatorStyle + "\n</style>");
                    _stringBuilder.AppendLine(string.Format("<title>Asset Validation Results: {0}</title>", DateTime.Now));
                    _stringBuilder.AppendLine(_script);
                    _stringBuilder.AppendLine("</head>");
                    _stringBuilder.AppendLine("<body>");
                    _stringBuilder.AppendLine("<table>");

                    foreach (var kvp in _validatorToHtmlStyle)
                    {
                        _stringBuilder.Append(@"<div class="".btn-group"">");
                        _stringBuilder.Append(string.Format("<input type=\"button\" value=\"Hide {0}\" class=\"system button\" onclick=\"hide_class(this, '{0}', '{1}')\" />",
                            kvp.Key, kvp.Key.ToLower()));
                        _stringBuilder.Append(@"</div>");
                    }

                    // Table Begin and Column Headers
                    _stringBuilder.AppendLine("<tr><th>Validator</th><th>VLogType</th><th>Source</th><th>Message</th><th>ScenePath</th><th>ObjectPath</th></tr>");
                    break;
                case OutputFormat.Csv:
                    // CSV Header
                    _stringBuilder.AppendLine("Validator,VLogType,Source,Message,ScenePath,ObjectPath");
                    break;
                case OutputFormat.Text:
                    _stringBuilder.AppendLine("Validator    VLogType    Source    Message    ScenePath    ObjectPath");
                    break;
                case OutputFormat.None:
                    // Do nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AppendVLog(VLog vLog)
        {
            switch (_oFormat)
            {
                case OutputFormat.Html:
                    var classStyle = string.Empty;
                    if(_validatorToHtmlStyle != null)
                        _validatorToHtmlStyle.TryGetValue(vLog.validatorName, out classStyle);

                    _stringBuilder.AppendLine(string.Format("<tr {0}><th>{1}</th><th>{2}</th><th>{3}</th><th>{4}</th><th>{5}</th><th>{6}</th></tr>",
                        classStyle, vLog.validatorName, vLog.vLogType, vLog.source, vLog.message, vLog.scenePath, vLog.objectPath));
                    break;
                case OutputFormat.Csv:
                    _stringBuilder.AppendLine(string.Format("{0},{1},{2},{3},{4},{5}",
                        vLog.validatorName, vLog.vLogType, vLog.source, vLog.message, vLog.scenePath, vLog.objectPath));
                    break;
                case OutputFormat.Text:
                    _stringBuilder.AppendLine(string.Format("{0}    {1}    {2}    {3}    {4}    {5}",
                        vLog.validatorName, vLog.vLogType, vLog.source, vLog.message, vLog.scenePath, vLog.objectPath));
                    break;
                case OutputFormat.None:
                    // Do nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("OutputFormat [{0}] is not valid!", _oFormat));
            }
        }

        public void AppendFooter()
        {
            switch (_oFormat)
            {
                case OutputFormat.Html:
                    _stringBuilder.AppendLine("</table>");
                    _stringBuilder.AppendLine("</body>");
                    _stringBuilder.AppendLine("</html>");
                    break;
                case OutputFormat.Csv:
                    // Do Nothing
                    break;
                case OutputFormat.Text:
                    _stringBuilder.AppendLine("AssetValidator Complete!");
                    break;
                case OutputFormat.None:
                    // Do nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("OutputFormat [{0}] is not valid!", _oFormat));
            }
        }

        public void Flush()
        {
            if (_stringBuilder.Length == 0) return;

            File.WriteAllText(_fileName, _stringBuilder.ToString());
        }

        #region IDisposable

        public void Dispose()
        {
            // Do nothing
        }

        #endregion

        #region const_stlyes

        private const string _style = @"
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

        private const string _script = @"
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

        #endregion
    }
}

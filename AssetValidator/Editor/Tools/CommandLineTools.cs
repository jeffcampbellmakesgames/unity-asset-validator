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

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// Helper methods for working with command line processes
	/// </summary>
	public static class CommandLineTools
	{
		/// <summary>
		/// Returns a series of command line arguments indexed into a dictionary.
		/// </summary>
		/// <param name="delimiter"></param>
		/// <returns></returns>
		public static Dictionary<string, string> GetNamedCommandlineArguments(char delimiter)
		{
			var dict = new Dictionary<string, string>();
			var args = Environment.GetCommandLineArgs();
			for (var i = 0; i < args.Length; i++)
			{
				var splitArg = args[i].Split(delimiter);

				if (splitArg.Length <= 1)
				{
					continue;
				}

				dict.Add(splitArg[0], splitArg[1]);
			}

			return dict;
		}
	}
}

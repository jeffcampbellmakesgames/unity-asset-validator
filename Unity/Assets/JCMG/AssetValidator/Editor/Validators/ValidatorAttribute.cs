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
	/// <see cref="ValidatorAttribute"/> uniquely identifies a closed validator implementation by way of
	/// a unique <see cref="Symbol"/>.
	/// </summary>
	public class ValidatorAttribute : Attribute
	{
		/// <summary>
		/// A unique identifier for this validator.
		/// </summary>
		public string Symbol { get; }

		/// <summary>
		/// Identifies a Validator by way of a unique symbol.
		/// </summary>
		/// <param name="symbol"></param>
		public ValidatorAttribute(string symbol)
		{
			Symbol = symbol;
		}
	}
}

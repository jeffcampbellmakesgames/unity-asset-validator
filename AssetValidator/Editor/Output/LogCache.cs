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
using System.Collections;
using System.Collections.Generic;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="LogCache"/> represents a cache of logs from a validation session.
	/// </summary>
	internal sealed class LogCache : IReadOnlyList<ValidationLog>
	{
		private readonly IList<ValidationLog> _logs;

		public LogCache()
		{
			_logs = new List<ValidationLog>();
		}

		/// <summary>
		/// Adds a <see cref="ValidationLog"/> <paramref name="log"/> to the cache.
		/// </summary>
		/// <param name="log"></param>
		internal void OnLogCreated(ValidationLog log)
		{
			_logs.Add(log);
		}

		/// <summary>
		/// Clears all <see cref="ValidationLog"/>(s) from the cache.
		/// </summary>
		internal void ClearLogs()
		{
			_logs.Clear();
		}

		/// <summary>
		/// Returns true if there are any <see cref="ValidationLog"/>s, otherwise false.
		/// </summary>
		/// <returns></returns>
		internal bool HasLogs()
		{
			return _logs.Count > 0;
		}

		/// <summary>
		/// The total number of validation logs in the cache.
		/// </summary>
		public int Count => _logs.Count;

		/// <summary>
		/// Returns a <see cref="ValidationLog"/> present at position <paramref name="index"/> in the cache.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ValidationLog this[int index]
		{
			get { return _logs[index]; }
		}

		public IEnumerator<ValidationLog> GetEnumerator()
		{
			return _logs.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

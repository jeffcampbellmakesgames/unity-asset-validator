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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="SortedEnum{T}"/> is a read-only collection of Enum values sorted by their <see cref="int"/> value.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class SortedEnum<T> : IReadOnlyList<T>
	{
		private readonly List<T> _allTypeValues;

		/// <summary>
		/// Constructor to create a pre-filled <see cref="SortedEnum{T}"/>.
		/// </summary>
		public SortedEnum()
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException(EditorConstants.InvalidTypeWarning);
			}

			_allTypeValues = new List<T>((T[])Enum.GetValues(typeof(T)));
			_allTypeValues.Sort();
		}

		/// <summary>
		/// Returns all <typeparamref name="T"/> values greater than <typeparamref name="T"/>
		/// <paramref name="value"/>.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public IEnumerable<T> GetAllGreaterThan(T value)
		{
			var typeVal = Convert.ToInt32(value);
			return _allTypeValues.Where(x => typeVal < Convert.ToInt32(x));
		}

		/// <summary>
		/// Returns all <typeparamref name="T"/> values greater than or equal to <typeparamref name="T"/>
		/// <paramref name="value"/>.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public IEnumerable<T> GetAllGreaterThanOrEqualTo(T value)
		{
			var typeVal = Convert.ToInt32(value);
			return _allTypeValues.Where(x => typeVal <= Convert.ToInt32(x));
		}

		/// <summary>
		/// Returns all <typeparamref name="T"/> values lesser than <typeparamref name="T"/>
		/// <paramref name="value"/>.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public IEnumerable<T> GetAllLesserThan(T value)
		{
			var typeVal = Convert.ToInt32(value);
			return _allTypeValues.Where(x => typeVal > Convert.ToInt32(x));
		}

		/// <summary>
		/// Returns all <typeparamref name="T"/> values lesser than or equal to <typeparamref name="T"/>
		/// <paramref name="value"/>.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public IEnumerable<T> GetAllLesserThanOrEqualTo(T value)
		{
			var typeVal = Convert.ToInt32(value);
			return _allTypeValues.Where(x => typeVal >= Convert.ToInt32(x));
		}

		/// <summary>
		/// Returns a <see cref="IEnumerator{T}"/> of all <typeparamref name="T"/> values.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			return _allTypeValues.GetEnumerator();
		}

		/// <summary>
		/// Returns a <see cref="IEnumerator"/> of this <see cref="SortedEnum{T}"/> instance.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// The total count of <typeparamref name="T"/> values.
		/// </summary>
		public int Count => _allTypeValues.Count;

		/// <summary>
		/// Returns <typeparamref name="T" /> value at position <paramref name="index" />.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public T this[int index] => _allTypeValues[index];
	}
}

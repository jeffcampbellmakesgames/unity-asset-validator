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
using System.Linq;
using NUnit.Framework;

namespace JCMG.AssetValidator.Editor.Tests
{
	[TestFixture]
	public class SortedEnumTests
	{
		private enum SortedEnumTestType : byte
		{
			A = 0,
			B = 1,
			C = 2
		}

		private SortedEnum<SortedEnumTestType> _se;

		[SetUp]
		public void SetUp()
		{
			_se = new SortedEnum<SortedEnumTestType>();
		}

		[Test]
		public void AssertCanGetAllValuesGreaterThanA()
		{
			var items = _se.GetAllGreaterThan(SortedEnumTestType.A).ToList();

			Assert.IsTrue(items.Contains(SortedEnumTestType.B));
			Assert.IsTrue(items.Contains(SortedEnumTestType.C));
		}

		[Test]
		public void AssertCanGetAllValuesGreaterThanOrEqualToA()
		{
			var items = _se.GetAllGreaterThanOrEqualTo(SortedEnumTestType.A).ToList();

			Assert.IsTrue(items.Contains(SortedEnumTestType.A));
			Assert.IsTrue(items.Contains(SortedEnumTestType.B));
			Assert.IsTrue(items.Contains(SortedEnumTestType.C));
		}

		[Test]
		public void AssertCanGetAllValuesLessThanC()
		{
			var items = _se.GetAllLesserThan(SortedEnumTestType.C).ToList();

			Assert.IsTrue(items.Contains(SortedEnumTestType.A));
			Assert.IsTrue(items.Contains(SortedEnumTestType.B));
		}

		[Test]
		public void AssertCanGetAllValuesLessThanOrEqualToC()
		{
			var items = _se.GetAllLesserThanOrEqualTo(SortedEnumTestType.C).ToList();

			Assert.IsTrue(items.Contains(SortedEnumTestType.A));
			Assert.IsTrue(items.Contains(SortedEnumTestType.B));
			Assert.IsTrue(items.Contains(SortedEnumTestType.C));
		}
	}
}

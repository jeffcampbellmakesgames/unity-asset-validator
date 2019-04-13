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
using NUnit.Framework;

namespace JCMG.AssetValidator.Editor.Tests
{
	[TestFixture]
	public class ReflectionUtilityTests
	{
		private readonly List<A> allAssignableFromA = new List<A>
		{
			new A(), new B(), new C(), new D()
		};

		private readonly List<A> allSubclassesOfA = new List<A>
		{
			new B(), new C(), new D()
		};

		[Test]
		public void AssertReflectionUtilityCanDeriveAllNestedSubclasses()
		{
			var allReflectionDerivedSubclassesOfA = new List<A>();
			allReflectionDerivedSubclassesOfA.AddRange(ReflectionTools.GetAllDerivedInstancesOfType<A>());

			Assert.AreEqual(3, allReflectionDerivedSubclassesOfA.Count);
			for (var i = 0; i < allSubclassesOfA.Count; i++)
			{
				Assert.IsTrue(
					allReflectionDerivedSubclassesOfA.Exists(x => x.GetType() == allSubclassesOfA[i].GetType()));
			}
		}

		[Test]
		public void AssertThatReflectionUtilityCanDistinguishDerivedTypesThatImplementAnInterface()
		{
			// Test root interface
			var allTypesOfAThatImplementILetter = new List<A>();
			allTypesOfAThatImplementILetter.AddRange(ReflectionTools.GetAllInstancesOfTypeWithInterface<A, ILetter>());

			Assert.AreEqual(4, allTypesOfAThatImplementILetter.Count);
			for (var i = 0; i < allAssignableFromA.Count; i++)
			{
				Assert.IsTrue(allAssignableFromA.Exists(x => x.GetType() == allAssignableFromA[i].GetType()));
			}

			// Test nested interface
			var allSubclassesOfAWithInterfaceIFavoriteLetter = new List<A>
			{
				new C(), new D()
			};

			var reflectionDerivedTypesOfAWithInterfaceIFavoriteLetter = new List<A>();
			reflectionDerivedTypesOfAWithInterfaceIFavoriteLetter.AddRange(ReflectionTools
				.GetAllInstancesOfTypeWithInterface<A, IFavoriteLetter>());

			Assert.AreEqual(2, reflectionDerivedTypesOfAWithInterfaceIFavoriteLetter.Count);
			for (var i = 0; i < allSubclassesOfAWithInterfaceIFavoriteLetter.Count; i++)
			{
				Assert.IsTrue(reflectionDerivedTypesOfAWithInterfaceIFavoriteLetter.Exists(x =>
					x.GetType() == allSubclassesOfAWithInterfaceIFavoriteLetter[i].GetType()));
			}
		}

		[Test]
		public void AssertThatReflectionUtilityCanFindTypeInstancesWithSpecificAttribute()
		{
			// All classes with Letter attribute
			var allTypesWithAttribute = new List<A>
			{
				new B(), new D()
			};

			var reflectionTypesOfAWithAttributeLetter = new List<A>();
			reflectionTypesOfAWithAttributeLetter.AddRange(ReflectionTools
				.GetAllInstancesOfTypeWithAttribute<A, LetterAttribute>());

			Assert.AreEqual(2, reflectionTypesOfAWithAttributeLetter.Count);
			for (var i = 0; i < allTypesWithAttribute.Count; i++)
			{
				Assert.IsTrue(reflectionTypesOfAWithAttributeLetter.Exists(x =>
					x.GetType() == allTypesWithAttribute[i].GetType()));
			}
		}

		[Test]
		public void AssertThatReflectionUtilityCanFindTypesWithSpecificAttribute()
		{
			// All classes with Letter attribute
			var allTypesWithAttribute = new List<Type>
			{
				typeof(B), typeof(D)
			};

			var reflectionTypesOfAWithAttributeLetter = new List<Type>();
			reflectionTypesOfAWithAttributeLetter.AddRange(ReflectionTools
				.GetAllDerivedTypesOfTypeWithAttribute<A, LetterAttribute>());

			Assert.AreEqual(2, reflectionTypesOfAWithAttributeLetter.Count);
			for (var i = 0; i < allTypesWithAttribute.Count; i++)
			{
				Assert.IsTrue(reflectionTypesOfAWithAttributeLetter.Exists(x => x == allTypesWithAttribute[i]));
			}
		}

		#region TestClasses

		private interface ILetter
		{
		}

		private interface IFavoriteLetter
		{
		}

		private class LetterAttribute : Attribute
		{
		}

		private class A : ILetter
		{
		}

		[Letter]
		private class B : A
		{
		}

		private class C : A, IFavoriteLetter
		{
		}

		private class D : B, IFavoriteLetter
		{
		}

		#endregion
	}
}

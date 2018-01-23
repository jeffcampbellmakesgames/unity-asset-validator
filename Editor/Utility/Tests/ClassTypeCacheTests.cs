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
using JCMG.AssetValidator.Editor.Meta;
using NUnit.Framework;
using System;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Utility.Tests
{
    [TestFixture]
    public class ClassTypeCacheTests
    {
        private ClassTypeCache classTypeCache;

        [SetUp]
        public void Setup()
        {
            classTypeCache = new ClassTypeCache();
        }

        [Test]
        public void AssertThatTypeCacheCanProperlyFindAndCacheTypes()
        {
            classTypeCache.AddType<CTCValidatedEntity>();

            Assert.AreEqual(2, classTypeCache.Count);
            Assert.AreEqual(typeof(CTCTestValidatedEntity), classTypeCache[0]);
        }

        [Test]
        public void AssertThatTypeCacheCanProperlyFindAndCacheTypesWithAttributes()
        {
            classTypeCache.AddTypeWithAttribute<CTCValidatedEntity, TestClassAttribute>();

            Assert.AreEqual(1, classTypeCache.Count);
            Assert.AreEqual(typeof(CTCTestValidatedEntity), classTypeCache[0]);
        }

        [Test]
        public void AssertThatTypeCacheCanIgnoreClassTypes()
        {
            classTypeCache.IgnoreType<IgnoredCTCTestValidatedEntity>();
            classTypeCache.AddTypeWithAttribute<CTCValidatedEntity, TestClassAttribute>();

            Assert.AreEqual(1, classTypeCache.Count);
            Assert.AreEqual(typeof(CTCTestValidatedEntity), classTypeCache[0]);
        }

        [Test]
        public void AssertThatTypeCacheCanIgnoreAttributeTypes()
        {
            classTypeCache.IgnoreType<IgnoredCTCTestValidatedEntity>();
            classTypeCache.IgnoreAttribute<TestClassAttribute>();
            classTypeCache.AddTypeWithAttribute<CTCValidatedEntity, TestClassAttribute>();

            // Because we've added VValidateAttribute as an ignored attribute, we shouldn't find any types
            Assert.AreEqual(0, classTypeCache.Count);
        }

        public abstract class CTCValidatedEntity : MonoBehaviour { }

        [AttributeUsage(AttributeTargets.Class)]
        public class TestClassAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Field)]
        public class TestFieldAttribute : Attribute { }

        [TestClass]
        public class CTCTestValidatedEntity : CTCValidatedEntity
        {
            [TestField]
            public GameObject PublicRef;
        }

        public class IgnoredCTCTestValidatedEntity : CTCTestValidatedEntity
        {

        }
    }
}

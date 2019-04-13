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
using System.Linq;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// A collection of non-abstract instance types.
	/// </summary>
	internal class ClassTypeCache
	{
		/// <summary>
		/// All cached types
		/// </summary>
		private readonly List<Type> _types;

		/// <summary>
		/// Anything that is or derives from these types is not included
		/// </summary>
		private readonly List<Type> _ignoreClassTypes;

		/// <summary>
		/// Anything that has or derives a types that has these attributes is not included
		/// </summary>
		private readonly List<Type> _ignoreAttributeTypes;

		public ClassTypeCache()
		{
			_types = new List<Type>();
			_ignoreClassTypes = new List<Type>();
			_ignoreAttributeTypes = new List<Type>();
		}

		/// <summary>
		/// Adds instance <see cref="Type"/> <typeparamref name="T"/> and any derived <see cref="Type"/>(s) of
		/// <typeparamref name="T"/> where they are non-abstract, have a default constructor, and are not
		/// included in ignored types.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void AddType<T>() where T : class
		{
			var types = ReflectionTools.GetAllDerivedTypesOfType<T>();
			foreach (var type in types)
			{
				if (IsIgnoredType(type))
				{
					continue;
				}

				_types.Add(type);
			}
		}

		/// <summary>
		/// Adds instance <see cref="Type"/> <typeparamref name="T"/> and any derived <see cref="Type"/>(s) of
		/// <typeparamref name="T"/> where they are non-abstract, have a default constructor, are not
		/// included in ignored types, and have a class attribute of <typeparamref name="TV"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TV"></typeparam>
		public void AddTypeWithAttribute<T, TV>() where T : class where TV : Attribute
		{
			var types = ReflectionTools.GetAllDerivedTypesOfTypeWithAttribute<T, TV>(false);
			foreach (var type in types)
			{
				if (IsIgnoredType(type))
				{
					continue;
				}

				_types.Add(type);
			}
		}

		/// <summary>
		/// Add a class <see cref="Type"/> <typeparamref name="T"/> to be excluded from caching even if it
		/// meets other qualifications when attempted to be added via <seealso cref="AddType{T}"/> or
		/// <seealso cref="AddTypeWithAttribute{T,TV}"/> or if it has already been added.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void IgnoreType<T>() where T : class
		{
			var ignoreType = typeof(T);
			if (!_ignoreClassTypes.Contains(ignoreType))
			{
				_ignoreClassTypes.Add(ignoreType);
			}

			for (var index = _types.Count - 1; index >= 0; index--)
			{
				var type = _types[index];
				if (type.IsAssignableFrom(ignoreType))
				{
					_types.Remove(type);
				}
			}
		}

		/// <summary>
		/// Adds an attribute <typeparamref name="T"/> on a class to be excluded from caching even if it
		/// meets other qualifications when attempted to be added via <seealso cref="AddType{T}"/> or
		/// <seealso cref="AddTypeWithAttribute{T,TV}"/> or if it has already been added.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void IgnoreAttribute<T>() where T : Attribute
		{
			var ignoreAttrType = typeof(T);
			if (!_ignoreAttributeTypes.Contains(ignoreAttrType))
			{
				_ignoreAttributeTypes.Add(ignoreAttrType);
			}

			for (var index = _types.Count - 1; index >= 0; index--)
			{
				var type = _types[index];
				if (type.IsDefined(ignoreAttrType, true))
				{
					_types.Remove(type);
				}
			}
		}

		/// <summary>
		/// Completely clears the class cache of all tracked or ignored <see cref="Type"/>(s) or
		/// class <see cref="Attribute"/>(s).
		/// </summary>
		public void Clear()
		{
			_types.Clear();
			_ignoreAttributeTypes.Clear();
			_ignoreClassTypes.Clear();
		}

		/// <summary>
		/// The total number of tracked types in the cache.
		/// </summary>
		public int Count
		{
			get { return _types.Count; }
		}

		/// <summary>
		/// Returns tracked <see cref="Type"/> at position <paramref name="index"/> in the cache.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Type this[int index]
		{
			get { return _types[index]; }
		}

		/// <summary>
		/// Is this a type we are supposed to ignore either because it is or derives from an ignored
		/// <see cref="Type"/> or because it is decorated with an ignored class <see cref="Attribute"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private bool IsIgnoredType(Type type)
		{
			if (_ignoreClassTypes.Any(x => type.IsSubclassOf(x) || type == x))
			{
				return true;
			}

			if (_ignoreAttributeTypes.Any(x => type.GetCustomAttributes(x, false).Length > 0))
			{
				return true;
			}

			return false;
		}
	}
}

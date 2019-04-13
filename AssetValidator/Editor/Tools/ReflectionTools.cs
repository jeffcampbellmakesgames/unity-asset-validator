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
using System.Runtime.Serialization;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// Helper methods for introspecting code via reflection.
	/// </summary>
	public static class ReflectionTools
	{
		/// <summary>
		/// Returns an IEnumerable of non-abstract, non-generic class instances of <see cref="Type"/>(s)
		/// derived from <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetAllDerivedInstancesOfType<T>() where T : class
		{
			var objects = new List<T>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				foreach (var type in assembly.GetTypes()
					.Where(myType => myType.IsClass &&
					                 !myType.IsAbstract &&
					                 !myType.IsGenericType &&
					                 myType.IsSubclassOf(typeof(T))))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of non-abstract, non-generic class instances of <see cref="Type"/>(s)
		/// derived from <typeparamref name="T"/> decorated with <see cref="Attribute"/>
		/// <typeparamref name="TV"/>.
		/// </summary>
		/// <typeparam name="T">The base type.</typeparam>
		/// <typeparam name="TV">The <see cref="Attribute"/> <see cref="Type"/> that derived classes of
		/// <typeparamref name="T"/> must be decorated with.</typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetAllDerivedInstancesOfTypeWithAttribute<T, TV>()
			where T : class
		{
			var objects = new List<T>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				foreach (var type in assembly.GetTypes()
					.Where(myType => myType.IsClass &&
					                 !myType.IsAbstract &&
					                 !myType.IsGenericType &&
					                 myType.IsSubclassOf(typeof(T)) &&
					                 myType.IsDefined(typeof(TV), true) &&
					                 myType.GetConstructor(Type.EmptyTypes) != null))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of non-abstract, non-generic class instances of <see cref="Type"/>(s)
		/// derived from <typeparamref name="T"/> implementing interface <typeparamref name="TV"/>.
		/// </summary>
		/// <typeparam name="T">The base type.</typeparam>
		/// <typeparam name="TV">The interface <see cref="Type"/> that derived classes of
		/// <typeparamref name="T"/> must implement.</typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetAllInstancesOfTypeWithInterface<T, TV>() where T : class
		{
			var objects = new List<T>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				foreach (var type in assembly.GetTypes()
					.Where(myType => myType.IsClass &&
					                 !myType.IsAbstract &&
					                 !myType.IsGenericType &&
					                 (myType.IsAssignableFrom(typeof(T)) || myType.IsSubclassOf(typeof(T))) &&
					                 myType.GetInterfaces().Contains(typeof(TV)) &&
					                 myType.GetConstructor(Type.EmptyTypes) != null))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of non-abstract, non-generic class instances of <see cref="Type"/>
		/// <typeparamref name="T"/> decorated with <see cref="Attribute"/> <typeparamref name="TV"/>.
		/// </summary>
		/// <typeparam name="T">The base type.</typeparam>
		/// <typeparam name="TV">The interface <see cref="Type"/> that classes of
		/// <typeparamref name="T"/> must implement.</typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetAllInstancesOfTypeWithAttribute<T, TV>() where T : class
		{
			var objects = new List<T>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				foreach (var type in assembly.GetTypes()
					.Where(myType => myType.IsClass &&
					                 !myType.IsAbstract &&
					                 !myType.IsGenericType &&
					                 (myType.IsAssignableFrom(typeof(T)) || myType.IsSubclassOf(typeof(T))) &&
					                 myType.IsDefined(typeof(TV), true) &&
					                 myType.GetConstructor(Type.EmptyTypes) != null))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of non-abstract, non-generic class instances of <see cref="Type"/>
		/// <typeparamref name="T"/> implementing interface <typeparamref name="TV"/>.
		/// </summary>
		/// <typeparam name="T">The base type.</typeparam>
		/// <typeparam name="TV">The interface <see cref="Type"/> that classes of <typeparamref name="T"/>
		/// must implement.</typeparam>
		/// <returns></returns>
		public static IEnumerable<T> GetAllDerivedInstancesOfTypeWithInterface<T, TV>() where T : class
		{
			var objects = new List<T>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				foreach (var type in assembly.GetTypes()
					.Where(myType => myType.IsClass &&
					                 !myType.IsAbstract &&
					                 !myType.IsGenericType &&
					                 myType.IsSubclassOf(typeof(T)) &&
					                 myType.GetInterfaces().Contains(typeof(TV)) &&
					                 myType.GetConstructor(Type.EmptyTypes) != null))
				{
					objects.Add((T)Activator.CreateInstance(type));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of non-abstract, non-generic class instances of <see cref="Type"/> that
		/// accept arguments <paramref name="constructorArgs"/> as arguments to a constructor.
		/// </summary>
		/// <typeparam name="T">The Type a class must derive from</typeparam>
		/// <param name="constructorArgs"></param>
		/// <returns></returns>
		public static IEnumerable<T> GetAllDerivedInstancesOfType<T>(params object[] constructorArgs)
			where T : class, new()
		{
			var objects = new List<T>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				foreach (var type in assembly.GetTypes()
					.Where(myType => myType.IsClass &&
					                 !myType.IsAbstract &&
					                 !myType.IsGenericType &&
					                 myType.IsSubclassOf(typeof(T))))
				{
					objects.Add((T)Activator.CreateInstance(type, constructorArgs));
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of <see cref="Type"/>(s) derived from <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The <see cref="Type"/> a class must derive from</typeparam>
		/// <returns></returns>
		public static IEnumerable<Type> GetAllDerivedTypesOfType<T>() where T : class
		{
			var objects = new List<Type>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				foreach (var type in assembly.GetTypes()
					.Where(myType => myType.IsClass &&
					                 myType.IsSubclassOf(typeof(T))))
				{
					objects.Add(type);
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns an IEnumerable of <see cref="Type"/>(s) derived from <typeparamref name="T"/> and implementing
		/// interface <typeparamref name="TV"/>.
		/// </summary>
		/// <typeparam name="T">The <see cref="Type"/> a class must derive from</typeparam>
		/// <typeparam name="TV">The interface a derived <see cref="Type"/> <typeparamref name="T"/>
		/// must implement.</typeparam>
		/// <returns></returns>
		public static IEnumerable<Type> GetAllDerivedTypesOfTypeWithAttribute<T, TV>()
			where T : class where TV : Attribute
		{
			return GetAllDerivedTypesOfTypeWithAttribute<T, TV>(true);
		}

		/// <summary>
		/// Returns an IEnumerable of <see cref="Type"/>(s) derived from <typeparamref name="T"/> and decorated
		/// with <see cref="Attribute"/> <typeparamref name="TV"/>. </summary>
		/// <typeparam name="T">The <see cref="Type"/> a class must derive from</typeparam>
		/// <typeparam name="TV">The <see cref="Attribute"/> a derived <see cref="Type"/> <typeparamref name="T"/>
		/// must be decorated with.</typeparam>
		/// <param name="inherit">Whether or not <see cref="Attribute"/> <typeparamref name="TV"/> can be
		/// inherited or not from a parent class.</param>
		/// <returns></returns>
		public static IEnumerable<Type> GetAllDerivedTypesOfTypeWithAttribute<T, TV>(bool inherit)
			where T : class where TV : Attribute
		{
			var objects = new List<Type>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (var i = 0; i < assemblies.Length; i++)
			{
				foreach (var type in assemblies[i].GetTypes()
					.Where(myType => (myType.IsClass &&
					                  myType.IsSubclassOf(typeof(T))) &&
					                  myType.IsDefined(typeof(TV), inherit)))
				{
					objects.Add(type);
				}
			}

			return objects;
		}

		/// <summary>
		/// Returns a <see cref="string"/> representation of <see cref="Enum"/> <see cref="Type"/>
		/// <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetEnumString<T>(T type)
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("Cannot convert non-enum Type to string.");
			}

			var enumType = typeof(T);
			var name = Enum.GetName(enumType, type);
			var fieldInfo = enumType.GetField(name);
			var enumMemberAttributes = (EnumMemberAttribute[])fieldInfo.GetCustomAttributes(
				typeof(EnumMemberAttribute),
				true);

			return enumMemberAttributes.Length > 0
				? enumMemberAttributes[0].Value
				: name;
		}
	}
}

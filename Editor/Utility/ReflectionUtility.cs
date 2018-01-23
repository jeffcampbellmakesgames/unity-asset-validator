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
using System.Linq;
using System.Runtime.Serialization;

namespace JCMG.AssetValidator.Editor.Utility
{
    public static class ReflectionUtility
    {
        /// <summary>
        /// Returns an IEnumerable of class instances of Types derived from T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAllDerivedInstancesOfType<T>() where T : class, new()
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
        /// Returns an IEnumerable of class instances of Types derived from T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAllDerivedInstancesOfTypeWithAttribute<T, TV>() 
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
                                         myType.IsSubclassOf(typeof(T)) &&
                                         myType.IsDefined(typeof(TV), true)))
                {
                    objects.Add((T) Activator.CreateInstance(type));
                }
            }
            return objects;
        }

        /// <summary>
        /// Returns an IEnumerable of class instances of Type T that implement interface V
        /// </summary>
        /// <typeparam name="T">The Type a class must derive from</typeparam>
        /// <typeparam name="TV">The Type a class must implement</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAllInstancesOfTypeWithInterface<T, TV>() where T : class, new()
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
                                         myType.GetInterfaces().Contains(typeof(TV))))
                {
                    objects.Add((T) Activator.CreateInstance(type));
                }
            }
            return objects;
        }

        /// <summary>
        /// Returns an IEnumerable of class instances of Type T that implement interface V
        /// </summary>
        /// <typeparam name="T">The Type a class must derive from</typeparam>
        /// <typeparam name="TV">The Attribute a class must have</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAllInstancesOfTypeWithAttribute<T, TV>() where T : class, new()
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
                                         myType.IsDefined(typeof(TV), true)))
                {
                    objects.Add((T) Activator.CreateInstance(type));
                }
            }
            return objects;
        }

        /// <summary>
        /// Returns an IEnumerable of class instances of Types derived from T that implement V.
        /// </summary>
        /// <typeparam name="T">The Type a class must derive from</typeparam>
        /// <typeparam name="TV">the Type a class must implement</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAllDerivedInstancesOfTypeWithInterface<T, TV>() where T : class, new()
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
                                         myType.GetInterfaces().Contains(typeof(TV))))
                {
                    objects.Add((T) Activator.CreateInstance(type));
                }
            }
            return objects;
        }

        /// <summary>
        /// Returns an IEnumerable of class instances of Types derived from T that take arguments constructorArgs
        /// </summary>
        /// <typeparam name="T">The Type a class must derive from</typeparam>
        /// <param name="constructorArgs"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllDerivedInstancesOfType<T>(params object[] constructorArgs) where T : class, new()
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
                    objects.Add((T) Activator.CreateInstance(type, constructorArgs));
                }
            }
            return objects;
        }

        /// <summary>
        /// Returns an IEnumerable of class instances of Types derived from T
        /// </summary>
        /// <typeparam name="T">The Type a class must derive from</typeparam>
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
        /// Returns an IEnumerable of class instances of Types derived from T
        /// </summary>
        /// <typeparam name="T">The Type a class must derive from</typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllDerivedTypesOfTypeWithAttribute<T, TV>(bool inherit = true) 
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

        public static string ToEnumString<T>(T type)
        {
            var enumType = typeof(T);
            var name = Enum.GetName(enumType, type);
            var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
            return enumMemberAttribute.Value;
        }
    }
}
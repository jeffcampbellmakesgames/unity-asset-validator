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
using UnityEditor;
using UnityEngine;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// Helper methods for dealing with <see cref="Object"/> instances.
	/// </summary>
	public static class ObjectTools
	{
		public static string GetObjectPath(Object obj)
		{
			// if this is an asset in the project, return the relative asset path.
			if (AssetDatabase.GetAssetPath(obj) != string.Empty)
			{
				return AssetDatabase.GetAssetPath(obj);
			}

			// Otherwise if this is a Component or GameObject in a scene, return
			// the hierarchical scene path.
			var objComponent = (obj as Component);
			if (objComponent != null)
			{
				return objComponent.transform.GetPath();
			}

			var objGameObject = obj as GameObject;
			if (objGameObject != null)
			{
				return objGameObject.transform.GetPath();
			}

			// Otherwise return an empty string.
			return string.Empty;
		}

		/// <summary>
		/// Returns true if the <see cref="Object"/> <paramref name="obj"/> is in the Unity Assets folder,
		/// otherwise returns false.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsProjectReference(Object obj)
		{
			var assetPath = AssetDatabase.GetAssetPath(obj);
			return !string.IsNullOrEmpty(assetPath);
		}

		/// <summary>
		/// Returns true if the <see cref="Object"/> <paramref name="obj"/> is in a Scene, otherwise
		/// returns false.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsSceneReference(Object obj)
		{
			return !IsProjectReference(obj);
		}

		/// <summary>
		/// Returns true if the <see cref="Object"/> <paramref name="obj"/> is null, otherwise returns false.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsNullReference(Object obj)
		{
			return obj == null ||
			       obj.ToString() == EditorConstants.NullGameObject;
		}
	}
}

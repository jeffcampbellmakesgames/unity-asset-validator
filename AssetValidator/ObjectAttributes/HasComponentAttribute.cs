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
using UnityEngine;

namespace JCMG.AssetValidator
{
	/// <summary>
	/// Any <see cref="MonoBehaviour"/> decorated with <see cref="HasComponentAttribute"/> must have the
	/// described component type directly on it or optionally on a child object.
	/// </summary>
	public sealed class HasComponentAttribute : ValidateAttribute
	{
		/// <summary>
		/// The <see cref="Component"/> type(s) that should be present on the decorated
		/// <see cref="MonoBehaviour"/>
		/// </summary>
		public Type[] TargetTypes { get; }

		/// <summary>
		/// Whether or not the <see cref="TargetTypes"/> can be present on a child <see cref="GameObject"/>
		/// or not.
		/// </summary>
		public bool CanBeOnChildObject { get; }

		/// <summary>
		/// Whether or not the <see cref="TargetTypes"/> can be present on a parent <see cref="GameObject"/>
		/// or not.
		/// </summary>
		public bool CanBeOnParentObject { get; }

		/// <summary>
		/// Empty-constructor used for reflection only.
		/// </summary>
		public HasComponentAttribute()
		{
			TargetTypes = new Type[0];
		}

		/// <summary>
		/// Constructor that specifies an array of <see cref="Component"/> <see cref="Type"/>(s) present on that
		/// <see cref="MonoBehaviour"/>'s <see cref="GameObject"/>.
		/// </summary>
		/// <param name="types">The required <see cref="Component"/> <see cref="Type"/>(s) on this
		/// <see cref="MonoBehaviour"/>'s <see cref="GameObject"/>.</param>
		public HasComponentAttribute(params Type[] types)
		{
			TargetTypes = types;
			CanBeOnChildObject = false;
			CanBeOnParentObject = false;
		}

		/// <summary>
		/// Constructor that specifies an array of <see cref="Component"/> <see cref="Type"/>(s) present on that
		/// <see cref="MonoBehaviour"/> <see cref="GameObject"/>.
		/// </summary>
		/// <param name="canBeOnChildObject">Whether or not the required <see cref="Component"/>
		/// <see cref="Type"/> can be on a parent <see cref="GameObject"/> or not.</param>
		///
		/// <param name="canBeOnParentObject">Whether or not the required <see cref="Component"/>
		/// <see cref="Type"/> can be on a child <see cref="GameObject"/> or not.</param>
		///
		/// <param name="types">The required <see cref="Component"/> <see cref="Type"/>(s) on this
		/// <see cref="MonoBehaviour"/>'s <see cref="GameObject"/>.</param>
		public HasComponentAttribute(bool canBeOnChildObject, bool canBeOnParentObject, params Type[] types)
		{
			TargetTypes = types;
			CanBeOnChildObject = canBeOnChildObject;
			CanBeOnParentObject = canBeOnParentObject;
		}
	}
}

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
using System.Diagnostics;
using UnityEngine;

namespace JCMG.AssetValidator
{
	/// <summary>
	/// <see cref="ValidateAttribute"/> is used by the validator editor tools to determine which
	/// <see cref="MonoBehaviour"/> or <see cref="ScriptableObject"/> classes should be targeted for
	/// validation. It is necessary when using Field Validation Attributes that <see cref="ValidateAttribute"/>
	/// is on the class whose Fields are being decorated. It is only available in the editor and will be
	/// conditionally compiled out at runtime.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class), Conditional("UNITY_EDITOR")]
	public class ValidateAttribute : Attribute
	{
		/// <summary>
		/// The <see cref="UnityTargetType"/> of object that this attribute should be used on.
		/// </summary>
		public UnityTargetType UnityTargetType { get; }

		public ValidateAttribute()
		{
			UnityTargetType = UnityTargetType.MonoBehavior;
		}

		/// <summary>
		/// Constructor to allow specifying the <see cref="UnityTargetType"/> of object that this should be
		/// validating.
		/// </summary>
		/// <param name="unityTargetType">The <see cref="UnityTargetType"/> of object that this attribute should be used on.</param>
		protected ValidateAttribute(UnityTargetType unityTargetType)
		{
			UnityTargetType = unityTargetType;
		}
	}
}

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
using System.Reflection;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// A <see cref="FieldValidatorBase"/> is the abstract base class for any validator that is inspecting
	/// fields on a <see cref="Object"/> type instance.
	/// </summary>
	public abstract class FieldValidatorBase : AbstractInstanceValidator
	{
		private const string NoFieldTargetAttributeWarning =
			"Subclasses of BaseVFieldValidator are required to be decorated with a VFieldTargetAttribute to " +
			"determine what VFieldAttribute they are validating";

		/// <summary>
		/// The binding flags for fields that can be validated on an instance. This includes both public,
		/// private, and protected instance fields.
		/// </summary>
		private const BindingFlags DefaultFieldFlags = BindingFlags.NonPublic |
		                                               BindingFlags.Public |
		                                               BindingFlags.Instance;
		/// <summary>
		/// The base <see cref="FieldValidatorAttribute"/> type.
		/// </summary>
		private static readonly Type FieldTargetAttrType = typeof(FieldValidatorAttribute);

		protected FieldValidatorBase()
		{
			var t = GetType();
			var vFieldTargets = (FieldValidatorAttribute[])t.GetCustomAttributes(FieldTargetAttrType, false);

			Assert.IsTrue(vFieldTargets.Length > 0, NoFieldTargetAttributeWarning);

			_typeToTrack = vFieldTargets[0].TargetType;
		}

		/// <summary>
		/// Returns true if any of <see cref="Object"/> <paramref name="obj"/>'s fields pertain to this
		/// validator, otherwise returns false.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public sealed override bool AppliesTo(Object obj)
		{
			var fields = GetFieldInfosApplyTo(obj);
			return fields.Any();
		}

		/// <summary>
		/// Returns true if any of <see cref="Type"/> <paramref name="type"/>'s fields pertain to this
		/// validator, otherwise returns false.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public sealed override bool AppliesTo(Type type)
		{
			var fields = GetFieldInfosApplyTo(type);
			return fields.Any();
		}

		/// <summary>
		/// Returns all <see cref="FieldInfo"/> instances on <see cref="System.object"/> <paramref name="obj"/>
		/// that pertain to this validator.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public IEnumerable<FieldInfo> GetFieldInfosApplyTo(object obj)
		{
			return GetFieldInfosApplyTo(obj.GetType());
		}

		/// <summary>
		/// Returns all <see cref="FieldInfo"/> instances on <see cref="Type"/> <paramref name="type"/>
		/// that pertain to this validator.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IEnumerable<FieldInfo> GetFieldInfosApplyTo(Type type)
		{
			return type.GetFields(DefaultFieldFlags).Where(TypeHasTrackedAttributeType);
		}

		/// <summary>
		/// Returns true if a given <see cref="FieldInfo"/> <paramref name="fieldInfo"/> is tracked by this
		/// validator due to it being decorated with the relevant <see cref="AssetValidator.FieldAttribute"/>
		/// type.
		/// </summary>
		/// <param name="fieldInfo"></param>
		/// <returns></returns>
		private bool TypeHasTrackedAttributeType(FieldInfo fieldInfo)
		{
			return fieldInfo.GetCustomAttributes(_typeToTrack, false).Length > 0;
		}
	}
}

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
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="ObjectValidatorBase"/> is a abstract validator class intended for validating an object
	/// instance deriving from either <see cref="MonoBehaviour"/> or <see cref="ScriptableObject"/>.
	/// </summary>
	public abstract class ObjectValidatorBase : AbstractInstanceValidator
	{
		private readonly UnityTargetType _targetTypeType;

		private const string HasObjectValidatorWarning =
			"Subclasses of ObjectValidatorBase are required to be decorated with a ObjectValidatorAttribute to " +
			"determine what type they are validating";

		protected ObjectValidatorBase()
		{
			var vObjectTargets = (ObjectValidatorAttribute[])GetType().GetCustomAttributes(typeof(ObjectValidatorAttribute), true);
			Assert.IsFalse(vObjectTargets.Length == 0, HasObjectValidatorWarning);

			_typeToTrack = vObjectTargets[0].TargetType;
			_targetTypeType = vObjectTargets[0].TargetAttribute.UnityTargetType;
		}

		public sealed override bool AppliesTo(Object obj)
		{
			return AppliesTo(obj.GetType());
		}

		public sealed override bool AppliesTo(Type type)
		{
			return HasRelevantAttribute(type) && DerivesFromCorrectTargetType(type);
		}

		private bool HasRelevantAttribute(Type type)
		{
			var attr = type.GetCustomAttributes(_typeToTrack, true);
			return attr.Length > 0;
		}

		private bool DerivesFromCorrectTargetType(Type type)
		{
			switch (_targetTypeType)
			{
				case UnityTargetType.MonoBehavior:
					return type.IsSubclassOf(typeof(MonoBehaviour));

				case UnityTargetType.ScriptableObject:
					return type.IsSubclassOf(typeof(ScriptableObject));
				default:
					throw new NotImplementedException(Enum.GetName(typeof(UnityTargetType), _targetTypeType));
			}
		}
	}
}

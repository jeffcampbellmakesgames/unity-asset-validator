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
using UnityEngine.Assertions;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="ObjectValidatorAttribute"/> is a decorator meant for usage on <see cref="ObjectValidatorBase"/>
	/// derived implementations to uniquely identify them and a <see cref="ValidateAttribute"/> derived target
	/// type that it will validate.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ObjectValidatorAttribute : ValidatorAttribute
	{
		/// <summary>
		/// The target <see cref="ValidateAttribute"/>-derived type that a validator will track.
		/// </summary>
		public Type TargetType { get; }

		/// <summary>
		/// The <see cref="ValidateAttribute"/> instance of <see cref="TargetAttribute"/>.
		/// </summary>
		public ValidateAttribute TargetAttribute { get; }

		private const string MissingValidateAttributeWarning =
			"ObjectValidatorAttribute must target an attribute deriving from ValidateAttribute.";

		public ObjectValidatorAttribute(string symbol, Type targetType) : base(symbol)
		{
			Assert.IsTrue(targetType.IsSubclassOf(typeof(ValidateAttribute)), MissingValidateAttributeWarning);

			TargetType = targetType;
			TargetAttribute = (ValidateAttribute)Activator.CreateInstance(TargetType);
		}
	}
}

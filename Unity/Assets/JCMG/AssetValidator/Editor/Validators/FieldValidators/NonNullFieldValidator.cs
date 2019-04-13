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
using UnityEngine;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="NonNullFieldValidator"/> is a validator meant to help ensure fields decorated with
	/// <see cref="NonNullAttribute"/> have values assigned.
	/// </summary>
	[FieldValidator("field_non_null_validator", typeof(NonNullAttribute))]
	[ValidatorDescription(
		"Uses the ```[NonNull]``` field attribute to ensure that the assigned reference is not null.")]
	[ValidatorExample(@"
/// <summary>
/// NonNullFieldComponent is a MonoBehavior derived class that has been marked as a [Validate]
/// target. Any fields of [NonNull] on it will be checked to see if it has a null value. If the
/// value of the field checked is null, a validation error will be dispatched.
/// </summary>
[Validate]
public class NonNullFieldComponent : MonoBehaviour
{
    [NonNull]
    public GameObject nullFieldExample;
}
")]
	public sealed class NonNullFieldValidator : FieldValidatorBase
	{
		private const string NullWarningFormat = "[{0}] has a null assignment for field [{1}]";

		public override bool Validate(Object obj)
		{
			var isValidated = true;
			var fields = GetFieldInfosApplyTo(obj);
			foreach (var field in fields)
			{
				var objValue = field.GetValue(obj);

				// If the field's type is a UnityEngine.Object derived type, cast it as such and check to see
				// if it is null. If not, continue.
				if (field.FieldType.IsSubclassOf(typeof(Object)))
				{
					var unityObjValue = objValue as Object;
					if (!ObjectTools.IsNullReference(unityObjValue))
					{
						continue;
					}
				}
				// Otherwise if it is not a UnityEngine.Object type, simple check for null and continue
				// if it has a value.
				else if(objValue != null)
				{
					continue;
				}

				DispatchLogEvent(
					obj,
					LogType.Error,
					string.Format(NullWarningFormat, obj.name, field.Name));

				isValidated = false;
			}

			return isValidated;
		}
	}
}

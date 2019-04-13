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
	[ObjectValidator("object_zero_children_validator", typeof(ZeroChildrenAttribute))]
	[ValidatorDescription(
		"Uses the ```[ZeroChildren]``` class attribute to ensure that the GameObject has zero children.")]
	[ValidatorExample(@"
/// <summary>
/// ZeroChildrenComponent is a MonoBehavior derived class that has been marked as a [Validate]
/// target due to [ZeroChildren] (a subclass attribute of [Validate]). All instances of ZeroChildrenComponent
/// will be found in any scenes searched and in the project on prefabs if searched. If any child GameObjects
/// are found on the ZeroChildrenComponent instance, a validation error will be dispatched.
/// </summary>
[ZeroChildren]
public class ZeroChildrenComponent : MonoBehaviour
{

}
")]
	public sealed class ZeroChildrenObjectValidator : ObjectValidatorBase
	{
		private const string InvalidTypeWarning = "[{0}] could not be cast to a MonoBehaviour.";
		private const string ChildObjectsPresentWarning = "[{0}] has one or more children when it should have zero.";

		public override bool Validate(Object obj)
		{
			var monoBehaviour = obj as MonoBehaviour;
			if (monoBehaviour == null)
			{
				DispatchLogEvent(obj, LogType.Warning, string.Format(InvalidTypeWarning, obj.name));

				return false;
			}

			var childCount = monoBehaviour.transform.childCount;
			if (childCount > 0)
			{
				DispatchLogEvent(obj, LogType.Error, string.Format(ChildObjectsPresentWarning, obj.name));
			}

			return childCount == 0;
		}
	}
}

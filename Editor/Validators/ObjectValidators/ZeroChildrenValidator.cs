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
using JCMG.AssetValidator.Editor.Validators.Output;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JCMG.AssetValidator.Editor.Validators.ObjectValidators
{
    [ObjectTarget("object_zero_children_validator", typeof(ZeroChildenAttribute))]
    [ValidatorDescription("Uses the ```[ZeroChilden]``` class attribute to ensure that the Gameobject has zero children.")]
    [ValidatorExample(@"
/// <summary>
/// ZeroChildrenComponent is a Monobehavior derived class that has been marked as a [Validate]
/// target due to [ZeroChilden] (a subclass attribute of [Validate]). All instances of ZeroChildrenComponent 
/// will be found in any scenes searched and in the project on prefabs if searched. If any child gameobjects
/// are found on the ZeroChildrenComponent instance, a validation error will be dispatched.
/// </summary>
[ZeroChilden]
public class ZeroChildrenComponent : MonoBehaviour
{

}
")]
    public class ZeroChildrenValidator : BaseObjectValidator
    {
        public override bool Validate(Object obj)
        {
            var monoBehaviour = obj as MonoBehaviour;
            if (monoBehaviour == null)
            {
                DispatchVLogEvent(obj, VLogType.Warning, string.Format("'{0}' could not be cast to a Monobehavior.", obj.name));

                return false;
            }

            var childCount = monoBehaviour.transform.childCount;
            if (childCount > 0)
                DispatchVLogEvent(obj, VLogType.Error, string.Format("'{0}' has one or more children when it should have zero.", obj.name));

            return childCount <= 0;
        }
    }
}

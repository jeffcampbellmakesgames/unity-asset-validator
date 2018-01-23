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
    [ObjectTarget("object_require_component_validator", typeof(HasComponentAttribute))]
    [ValidatorDescription("Uses the ```[RequireComponent]``` class attribute to ensure a required component's presence either on this object directly, on a " +
                          "parent or child object, or a combination thereof (i.e, on this object or on a child).")]
    [ValidatorExample(@"
/// <summary>
/// HasComponentExampleChild is a Monobehavior derived class that has been marked as a [Validate]
/// target due to [HasComponent] (a subclass attribute of [Validate]). All instances of HasComponentExampleChild 
/// will be found in any scenes searched and in the project on prefabs if searched. If the HasComponent type
/// (in this case HasComponentExampleParent) is not found either on the object or on a parent object a validation error
/// will be dispatched.
/// </summary>
[HasComponent(typeof(HasComponentExampleParent), false, true)]
public class HasComponentExampleChild : MonoBehaviour
{

}

/// <summary>
/// HasComponentExampleA is a Monobehavior derived class that has been marked as a [Validate]
/// target due to [HasComponent] (a subclass attribute of [Validate]). All instances of HasComponentExampleParent 
/// will be found in any scenes searched and in the project on prefabs if searched. If the HasComponent type
/// (in this case HasComponentExampleChild) is not found either on the object or on a child object a validation error
/// will be dispatched.
/// </summary>
[HasComponent(typeof(HasComponentExampleChild), true)]
public class HasComponentExampleParent : MonoBehaviour
{

}")]
    public class HasComponentValidator : BaseObjectValidator
    {
        public override bool Validate(Object obj)
        {
            var monoBehaviour = obj as MonoBehaviour;
            if (monoBehaviour == null)
            {
                DispatchVLogEvent(obj, VLogType.Warning, string.Format("'{0}' could not be cast to a Monobehavior.", obj.name));

                return false;
            }

            var allComponentsHaveBeenFound = true;
            var vReqAttrs = (HasComponentAttribute[])monoBehaviour.GetType().GetCustomAttributes(_typeToTrack, true);
            foreach (var vReqAttr in vReqAttrs)
            {
                var requiredTypes = vReqAttr.TargetTypes;
                foreach (var reqType in requiredTypes)
                {
                    var foundComponent = false;

                    if (vReqAttr.CanBeOnChildObject)
                    {
                        var component = monoBehaviour.GetComponentInChildren(reqType);
                        foundComponent = component != null;
                    }

                    if (!foundComponent && vReqAttr.CanBeOnParentObject)
                    {
                        var component = monoBehaviour.GetComponentInParent(reqType);
                        foundComponent = component != null;
                    }

                    if(!vReqAttr.CanBeOnChildObject && !vReqAttr.CanBeOnParentObject)
                    {
                        var component = monoBehaviour.GetComponent(reqType);
                        foundComponent = component != null;
                    }

                    allComponentsHaveBeenFound &= foundComponent;

                    if (!foundComponent)
                        DispatchVLogEvent(obj, VLogType.Error, string.Format("'{0}' does not have a component of type [{1}]", obj.name, reqType.Name));
                }
            }

            return allComponentsHaveBeenFound;
        }
    }
}

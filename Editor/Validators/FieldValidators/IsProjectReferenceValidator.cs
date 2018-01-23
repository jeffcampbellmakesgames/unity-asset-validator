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
using JCMG.AssetValidator.Editor.Utility;
using JCMG.AssetValidator.Editor.Validators.Output;
using UnityEngine;

namespace JCMG.AssetValidator.Editor.Validators.FieldValidators
{
    [FieldTarget("field_project_reference_validator", typeof(IsProjectReferenceAttribute))]
    [ValidatorDescription("Uses the ```[IsProjectReference]``` field attribute to ensure that the assigned reference is not null and refers to an asset in " +
                          "the project and not an instance in the scene.")]
    [ValidatorExample(@"
/// <summary>
/// ProjectReferenceComponent is a Monobehavior derived class that has been marked as a [Validate]
/// target. All instances of ProjectReferenceComponent will be found in any scenes searched and in the 
/// project on prefabs if searched. Any fields with [IsProjectReference] will be checked to ensure they
/// are not null and that the Object it refers to is a project asset.
/// </summary>
[Validate]
public class ProjectReferenceComponent : MonoBehaviour
{
    [IsProjectReference]
    public GameObject projectReferenceObject;
}")]
    public class IsProjectReferenceValidator : BaseFieldValidator
    {
        public override bool Validate (Object obj)
        {
            var isValidated = true;
            var fields = GetFieldInfosApplyTo(obj);

            foreach (var field in fields)
            {
                var value = field.GetValue(obj);
                if (value == null)
                {
                    DispatchVLogEvent(obj, VLogType.Error, string.Format("Field [{0}] on Object [{1}] is null when it should be a" +
                                                           " reference to a project asset", field, obj.name)); 
                    isValidated = false;
                    continue;
                }

                var unityObject = value as Object;

                if(ObjectUtility.IsNullReference(unityObject))
                {
                    DispatchVLogEvent(obj, VLogType.Warning, string.Format("Field [{0}] on Object [{1}] should not have a VIsProjectReference " +
                                                             "attribute as it does not derive from UnityEngine.Object", field, obj.name));
                    continue;
                }

                if(!ObjectUtility.IsProjectReference(unityObject))
                {
                    DispatchVLogEvent(obj, VLogType.Error, string.Format("Field [{0}] on Object [{1}] does not refer to a project asset " +
                                                           "when it should", field, obj.name));
                    isValidated = false;
                }
            }

            return isValidated;
        }
    }
}


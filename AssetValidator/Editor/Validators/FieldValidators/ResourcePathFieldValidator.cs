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
	/// <see cref="ResourcePathFieldValidator"/> is a validator that helps to ensure fields decorated with
	/// <see cref="ResourcePathAttribute"/> when calling their <see cref="object.ToString"/> method resolve to
	/// a resource path for a non-null asset.
	/// </summary>
	[FieldValidator("field_resource_path_validator", typeof(ResourcePathAttribute))]
	[ValidatorDescription(
		"Uses the ```[ResourcePath]``` field attribute to ensure that the ToString result from this field results in a reference" +
		"to an object that could be loaded from the Resources folder using Resources.Load.")]
	[ValidatorExample(@"
/// <summary>
/// ResourcePathComponent is a MonoBehavior derived class that has been marked as a [Validate]
/// target. Any fields of [ResourcePath] on it will be used in an attempt to load an object from
/// the project using Resources.Load where the path will be the result of the ToString method of
/// the field value.
/// </summary>
[Validate]
public class ResourcePathComponent : MonoBehaviour
{
    [ResourcePath]
    public string resourcePath;

    [ResourcePath]
    public ResourcePathObjectExample resourcePathObject = new ResourcePathObjectExample()
    {
        folder = ""hero_icon"",
        item = ""warrior_icon""
    };
}

/// <summary>
/// When a [ResourcePath] is used on an object, its ToString method will be used to
/// get the resource path to be validated
/// </summary>
public class ResourcePathObjectExample
{
    public string folder;
    public string item;

    public override string ToString()
    {
        return string.Format(""{0}/{1}"", folder, item);
    }
}
")]
	public sealed class ResourcePathFieldValidator : FieldValidatorBase
	{
		public override bool Validate(Object obj)
		{
			var fields = GetFieldInfosApplyTo(obj);
			var isValidated = true;
			foreach (var fieldInfo in fields)
			{
				var value = fieldInfo.GetValue(obj);
				if (value == null)
				{
					isValidated = false;
					break;
				}

				var strValue = value.ToString();
				var resourceObj = Resources.Load(strValue);
				if (resourceObj != null)
				{
					continue;
				}

				isValidated = false;
				break;
			}

			return isValidated;
		}
	}
}

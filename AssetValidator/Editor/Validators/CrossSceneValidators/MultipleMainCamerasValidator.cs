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
	/// <see cref="MultipleMainCamerasValidator"/> is a cross-scene validator that validates that there is only
	/// one <see cref="Camera"/> component instance tagged as the "MainCamera".
	/// </summary>
	[Validator("cross_scene_multiple_cameras")]
	[ValidatorDescription(
		"Checks one or more Scenes to ensure that there is only one Camera tagged as the MainCamera.")]
	public sealed class MultipleMainCamerasValidator : EnsureComponentIsUniqueValidator<Camera>
	{
		private const string MAIN_CAMERA_TAG = "MainCamera";

		public override bool ShouldAddComponent(Camera obj)
		{
			return obj.CompareTag(MAIN_CAMERA_TAG);
		}

		public override string TargetTypeName
		{
			get { return "Camera Tagged as \"MainCamera\""; }
		}
	}
}

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
using UnityEngine.EventSystems;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="MultipleEventSystemsValidator"/> is a a cross-scene validator that helps validate whether or
	/// not there is a single EventSystem instance present in one or more scene(s).
	/// </summary>
	[Validator("cross_scene_multiple_event_systems")]
	[ValidatorDescription("Checks one or more Scenes to ensure that there is only one EventSystem.")]
	public sealed class MultipleEventSystemsValidator : EnsureComponentIsUniqueValidator<EventSystem>
	{
	}
}

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
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="MissingReferenceValidator"/> is a cross-scene validator that helps to find missing component
	/// references on <see cref="Object"/>(s).
	/// </summary>
	[Validator("cross_scene_missing_component_reference")]
	[ValidatorDescription(
		"Checks one or more Scenes to see if there are any missing Component references on every GameObject " +
		"and and their SerializedProperties in that scene.")]
	public sealed class MissingReferenceValidator : CrossSceneValidatorBase
	{
		private bool _foundMissingReferenceComponent;

		private const string MissingReferenceComponentError =
			"There is a missing component on Gameobject [{0}]";
		private const string MissingReferencePropertyError =
			"There is a missing component reference on Gameobject [{0}] on component [{1}] for property [{2}].";
		private const string FinalWarning =
			"Missing component references were found in the Scene(s) searched...";

		public override void Search()
		{
			var objects = Object.FindObjectsOfType<GameObject>();
			var currentScene = SceneManager.GetActiveScene().path;
			foreach (var obj in objects)
			{
				var components = obj.GetComponents<Component>();
				foreach (var c in components)
				{
					if (!c)
					{
						DispatchVLogEvent(
							obj,
							LogType.Error,
							string.Format(MissingReferenceComponentError, obj.name),
							currentScene);

						_foundMissingReferenceComponent = true;
					}
					else
					{
						var so = new SerializedObject(c);
						var sp = so.GetIterator();
						while (sp.NextVisible(true))
						{
							if (sp.propertyType != SerializedPropertyType.ObjectReference)
							{
								continue;
							}

							if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0)
							{
								DispatchVLogEvent(
									obj,
									LogType.Error,
									string.Format(MissingReferencePropertyError,
										obj.name,
										c.GetType().Name,
										ObjectNames.NicifyVariableName(sp.name)),
									currentScene);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Return true only if we did not find any missing
		/// </summary>
		/// <returns></returns>
		public override bool Validate()
		{
			if (_foundMissingReferenceComponent)
			{
				DispatchVLogEvent(new ValidationLog
				{
					logType = LogType.Error,
					source = LogSource.None,
					validatorName = TypeName,
					scenePath = string.Empty,
					objectPath = string.Empty,
					message = FinalWarning
				});
			}

			return !_foundMissingReferenceComponent;
		}
	}
}

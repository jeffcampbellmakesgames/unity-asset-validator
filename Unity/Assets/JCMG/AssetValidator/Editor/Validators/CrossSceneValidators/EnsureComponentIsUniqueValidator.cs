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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="EnsureComponentIsUniqueValidator{T}"/> is an abstract validator that should be derived
	/// from where there should only ever be one instance of a component present in one or more scene(s)
	/// searched.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[ValidatorDescription("This validator should be derived from where there should only ever be " +
	                      "one instance of a component present in the scenes searched.")]
	[ValidatorExample(@"
/// <summary>
/// FooComponent is a MonoBehavior derived component. There should only be
/// one FooComponent for any of the scenes searched.
/// </summary>
public class FooComponent : MonoBehaviour
{

}

/// <summary>
/// This is a subclass of EnsureComponentIsUniqueValidator; as the generic type is of FooComponent,
/// when this validator searches across one or more scenes, if it finds more than one instance of
/// FooComponent a validation warning will be dispatched per instance and an error noting that there
/// should only be one instance.
/// </summary>
public class EnsureFooComponentIsUniqueValidator : EnsureComponentIsUniqueValidator<FooComponent>
{

}
")]
	public abstract class EnsureComponentIsUniqueValidator<T> : CrossSceneValidatorBase
		where T : Component
	{
		/// <summary>
		/// The name of the <see cref="Component"/> <see cref="Type"/> that should be a unique instance
		/// across all scenes.
		/// </summary>
		public virtual string TargetTypeName
		{
			get { return _typeToTrack.Name; }
		}

		private readonly List<ValidationLog> _logs;
		private readonly Type _typeToTrack;

		private const string WarningFormat =
			"Scene at path [{0}] is not the only Scene to contain an [{1}]...";
		private const string FinalWarningFormat =
			"More than one Scene of the Scene(s) validated has an [{0}] present";

		protected EnsureComponentIsUniqueValidator()
		{
			_typeToTrack = typeof(T);
			_logs = new List<ValidationLog>();
		}

		public sealed override void Search()
		{
			var scenePath = SceneManager.GetActiveScene().path;
			var components = Object.FindObjectsOfType(_typeToTrack);
			if (components.Length <= 1)
			{
				return;
			}

			// Create a warning log for each T component found. These are
			// only used if more than one is found across all scenes
			for (var i = 0; i < components.Length; i++)
			{
				if (!ShouldAddComponent(components[i] as T))
				{
					continue;
				}

				var vLog = CreateVLog(
					components[i],
					LogType.Warning,
					string.Format(WarningFormat, scenePath, TargetTypeName),
					scenePath,
					LogSource.Scene);

				_logs.Add(vLog);
			}
		}

		/// <summary>
		/// Should <typeparamref name="T"/> <paramref name="obj"/> be included as a component instance
		/// that we want to track for uniqueness across one or more scene(s).
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public virtual bool ShouldAddComponent(T obj)
		{
			return true;
		}

		public sealed override bool Validate()
		{
			// If there is only one instance of a log for this type of component across all scenes,
			// return true as validation is successful.
			if (_logs.Count <= 1)
			{
				return true;
			}

			// Otherwise dispatch logs for all found non-unique components and return false.
			for (var i = 0; i < _logs.Count; i++)
			{
				DispatchVLogEvent(_logs[i]);
			}

			DispatchVLogEvent(new ValidationLog
			{
				logType = LogType.Error,
				source = LogSource.None,
				validatorName = TypeName,
				scenePath = string.Empty,
				objectPath = string.Empty,
				message = string.Format(FinalWarningFormat, _typeToTrack.Name)
			});

			return false;
		}
	}
}

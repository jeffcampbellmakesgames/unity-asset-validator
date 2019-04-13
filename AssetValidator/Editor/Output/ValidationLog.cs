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
using UnityEngine.SceneManagement;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="ValidationLog"/> represents a single validation log instance. It can be associated with a
	/// specific validator, object and/or scene, and area of the Unity project.
	/// </summary>
	public sealed class ValidationLog
	{
		/// <summary>
		/// The type of log.
		/// </summary>
		internal LogType logType;

		/// <summary>
		/// The general location where the log is associated with.
		/// </summary>
		internal LogSource source;

		/// <summary>
		/// The Type name of the validator that this log was created from.
		/// </summary>
		internal string validatorName;

		/// <summary>
		/// The log message.
		/// </summary>
		internal string message;

		/// <summary>
		/// The relative path to a scene starting from the Unity Assets folder.
		/// </summary>
		internal string scenePath;

		/// <summary>
		/// The scene object hierarchical path value.
		/// </summary>
		internal string objectPath;

		internal ValidationLog()
		{
		}

		/// <summary>
		/// Returns a human-readable description of the <see cref="LogSource"/> <see cref="source"/>.
		/// </summary>
		/// <returns></returns>
		public string GetSourceDescription()
		{
			switch (source)
			{
				case LogSource.Scene:
					return scenePath;

				case LogSource.Project:
					return EditorConstants.ProjectDescription;

				default:
					return EditorConstants.NoneDescription;
			}
		}

		/// <summary>
		/// Returns true or false if an object path is present.
		/// </summary>
		/// <returns></returns>
		public bool HasObjectPath()
		{
			return !string.IsNullOrEmpty(objectPath);
		}

		/// <summary>
		/// Can a scene be loaded using the <see cref="scenePath"/> of this log.
		/// </summary>
		/// <returns></returns>
		public bool CanLoadScene()
		{
			return !string.IsNullOrEmpty(scenePath) &&
			       SceneManager.GetActiveScene().path != scenePath;
		}

		/// <summary>
		/// Can the object relating to this log be pinged in the scene or Project Assets view?
		/// </summary>
		/// <returns></returns>
		public bool CanPingObject()
		{
			return HasObjectPath() &&
			       (source == LogSource.Scene &&
			        SceneManager.GetActiveScene().path == scenePath ||
			        source == LogSource.Project);
		}
	}
}

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
using Object = UnityEngine.Object;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// <see cref="ProjectValidatorBase"/> is fired once per project asset validation run (one search and
	/// validate).
	/// </summary>
	public abstract class ProjectValidatorBase
	{
		/// <summary>
		/// Invoked when this validator implementation has created a new <see cref="ValidationLog"/> instance.
		/// </summary>
		internal event Action<ValidationLog> LogCreated;

		private string _typeName;

		/// <summary>
		/// The human-readable name of the type.
		/// </summary>
		public string TypeName
		{
			get
			{
				if (string.IsNullOrEmpty(_typeName))
				{
					_typeName = GetType().Name;
				}

				return _typeName;
			}
		}

		/// <summary>
		/// Returns the number of items discovered that require validation.
		/// </summary>
		/// <returns></returns>
		public abstract int GetNumberOfResults();

		/// <summary>
		/// Search the current Scene and add the information gathered to a cache so that
		/// we can validate it in aggregate after the target Scene(s) have been searched.
		/// </summary>
		public abstract void Search();

		/// <summary>
		/// After all scenes have been searched, validate using the aggregated results.
		/// </summary>
		/// <returns></returns>
		public abstract bool Validate();

		/// <summary>
		/// Dispatches a <see cref="ValidationLog"/> instance via <see cref="LogCreated"/>.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="type"></param>
		/// <param name="message"></param>
		protected void DispatchLogEvent(
			Object obj,
			LogType type,
			string message)
		{
			DispatchLogEvent(
				obj,
				type,
				message,
				string.Empty,
				string.Empty);
		}

		/// <summary>
		/// Dispatches a <see cref="ValidationLog"/> instance via <see cref="LogCreated"/>.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="type"></param>
		/// <param name="message"></param>
		/// <param name="scenePath"></param>
		/// <param name="objectPath"></param>
		protected void DispatchLogEvent(
			Object obj,
			LogType type,
			string message,
			string scenePath,
			string objectPath)
		{
			LogCreated?.Invoke(new ValidationLog
			{
				logType = type,
				source = LogSource.Project,
				validatorName = TypeName,
				message = message,
				objectPath = string.IsNullOrEmpty(objectPath) ? ObjectTools.GetObjectPath(obj) : objectPath,
				scenePath = scenePath
			});
		}
	}
}

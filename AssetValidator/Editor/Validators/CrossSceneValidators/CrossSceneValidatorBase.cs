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
	/// <see cref="CrossSceneValidatorBase"/> is an abstract validator base class that searches for and
	/// aggregates information across multiple scenes and then provides validation based on that.
	/// </summary>
	public class CrossSceneValidatorBase
	{
		/// <summary>
		/// Invoked when this validator implementation has created a new <see cref="ValidationLog"/> instance.
		/// </summary>
		internal event Action<ValidationLog> LogCreated;

		/// <summary>
		/// The <see cref="TypeName"/> of this object.
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

		private string _typeName;

		/// <summary>
		/// Search the current Scene and add the information gathered to a cache so that
		/// we can validate it in aggregate after the target Scene(s) have been searched.
		/// </summary>
		public virtual void Search()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// After all scenes have been searched, validate using the aggregated results.
		/// </summary>
		/// <returns></returns>
		public virtual bool Validate()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Dispatches a <see cref="ValidationLog"/> instance via <see cref="LogCreated"/>.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="type"></param>
		/// <param name="message"></param>
		/// <param name="scenePath"></param>
		/// <param name="source"></param>
		protected ValidationLog CreateVLog(Object obj,
			LogType type,
			string message,
			string scenePath,
			LogSource source)
		{
			return new ValidationLog
			{
				logType = type,
				source = source,
				validatorName = TypeName,
				message = message,
				objectPath = ObjectTools.GetObjectPath(obj),
				scenePath = scenePath
			};
		}

		protected void DispatchVLogEvent(ValidationLog validationLog)
		{
			LogCreated?.Invoke(validationLog);
		}

		protected void DispatchVLogEvent(Object obj,
			LogType type,
			string message,
			string scenePath = "",
			LogSource source = LogSource.Scene)
		{
			LogCreated?.Invoke(new ValidationLog
			{
				logType = type,
				source = source,
				validatorName = TypeName,
				message = message,
				objectPath = ObjectTools.GetObjectPath(obj),
				scenePath = scenePath
			});
		}
	}
}

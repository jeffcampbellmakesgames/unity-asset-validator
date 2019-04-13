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
	/// <see cref="AbstractInstanceValidator"/> validates <see cref="Object"/> instances of a specific
	/// <see cref="Type"/>.
	/// </summary>
	public abstract class AbstractInstanceValidator
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

		protected Type _typeToTrack;

		/// <summary>
		/// Return true if this validator deems the <see cref="Object"/> <paramref name="obj"/> as passed
		/// validation, otherwise returns false.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public abstract bool Validate(Object obj);

		/// <summary>
		/// Returns true if this validator applies to <see cref="Object"/> <paramref name="obj"/>,
		/// otherwise returns false.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public abstract bool AppliesTo(Object obj);

		/// <summary>
		/// Returns true if this validator applies to <see cref="Type"/> <paramref name="type"/>,
		/// otherwise returns false.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public abstract bool AppliesTo(Type type);

		/// <summary>
		/// Dispatches a <see cref="ValidationLog"/> instance via <see cref="LogCreated"/>.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="type"></param>
		/// <param name="message"></param>
		protected void DispatchLogEvent(Object obj, LogType type, string message)
		{
			LogCreated?.Invoke(new ValidationLog
			{
				logType = type,
				validatorName = TypeName,
				message = message,
				objectPath = ObjectTools.GetObjectPath(obj)
			});
		}
	}
}

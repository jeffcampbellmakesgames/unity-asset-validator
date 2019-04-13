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
using System.Text;
using UnityEditor;
using UnityEngine;

namespace JCMG.AssetValidator.Editor
{
	/// <summary>
	/// Utility functions for drawing the scene validator window
	/// </summary>
	internal static class GraphicsTools
	{
		/// <summary>
		/// The <see cref="GUIStyle"/> for validation log row body background.
		/// </summary>
		public static readonly GUIStyle LogRowBodyBackground = "RL Background";

		/// <summary>
		/// The <see cref="GUIStyle"/> for validation log row header background.
		/// </summary>
		public static readonly GUIStyle LogRowHeaderBackground = "RL Header";

		private static readonly Color _grayColor = new Color(0.5f, 0.5f, 0.5f, 1f);
		private static Texture2D _grayTexture2D;

		/// <summary>
		/// Gray UI Texture
		/// </summary>
		public static Texture2D GrayTexture2D
		{
			get
			{
				if (_grayTexture2D == null)
				{
					_grayTexture2D = CreateUITexture(_grayColor);
				}

				return _grayTexture2D;
			}
		}

		/// <summary>
		/// Creates a 1x1 pixel texture of the passed <see cref="Color"/> <paramref name="color"/>.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		private static Texture2D CreateUITexture(Color color)
		{
			var tex = new Texture2D(1, 1);
			tex.SetPixel(0, 0, color);
			tex.Apply();

			return tex;
		}

		private static Texture2D warningIconSmall;

		/// <summary>
		/// The small warning icon texture.
		/// </summary>
		public static Texture2D WarningIconSmall
		{
			get
			{
				if (warningIconSmall == null)
				{
					warningIconSmall = (Texture2D)EditorGUIUtility.Load("icons/console.warnicon.sml.png");
				}

				return warningIconSmall;
			}
		}

		private static Texture2D infoIconSmall;

		/// <summary>
		/// The small info icon texture.
		/// </summary>
		public static Texture2D InfoIconSmall
		{
			get
			{
				if (infoIconSmall == null)
				{
					infoIconSmall = (Texture2D)EditorGUIUtility.Load("icons/console.infoicon.sml.png");
				}

				return infoIconSmall;
			}
		}


		/// <summary>
		/// The small error icon texture.
		/// </summary>
		public static Texture2D ErrorIconSmall
		{
			get
			{
				if (errorIconSmall == null)
				{
					errorIconSmall = (Texture2D)EditorGUIUtility.Load("icons/console.erroricon.sml.png");
				}

				return errorIconSmall;
			}
		}

		private static Texture2D errorIconSmall;

		private static readonly StringBuilder _stringBuilder = new StringBuilder();

		/// <summary>
		/// Returns the appropriate <see cref="GUIStyle"/> for the passed <see cref="ValidationLog"/>
		/// <paramref name="validationLog"/> based in its <see cref="LogType"/>. If null, the
		/// <see cref="LogType.Info"/> header style is returned.
		/// </summary>
		/// <param name="validationLog"></param>
		/// <returns></returns>
		public static GUIStyle GetLogHeaderStyle(ValidationLog validationLog)
		{
			GUIStyle style;

			var logType = validationLog == null ? LogType.Info : validationLog.logType;
			switch (logType)
			{
				case LogType.Warning:
					style = EditorGUIUtility.isProSkin ? EditorStyles.whiteBoldLabel : EditorStyles.boldLabel;
					break;

				case LogType.Error:
					style = EditorGUIUtility.isProSkin ? EditorStyles.boldLabel : EditorStyles.whiteBoldLabel;
					break;

				default:
					style = EditorGUIUtility.isProSkin ? EditorStyles.boldLabel : EditorStyles.whiteBoldLabel;
					break;
			}

			return style;
		}

		/// <summary>
		/// Returns the appropriate <see cref="Color"/> based on the passed <see cref="ValidationLog"/>
		/// <paramref name="log"/>'s <see cref="LogType"/>. If null, the <see cref="LogType.Info"/> header
		/// color is returned.
		/// </summary>
		/// <param name="log"></param>
		/// <returns></returns>
		public static Color GetLogHeaderColor(ValidationLog log)
		{
			Color color;
			var logType = log == null ? LogType.Info : log.logType;
			switch (logType)
			{
				case LogType.Warning:
					color = Color.yellow;
					break;

				case LogType.Error:
					color = Color.red;
					break;

				default:
					color = _grayColor;
					break;
			}

			return color;
		}

		/// <summary>
		/// Returns a multi-line string created from <see cref="string"/> <paramref name="message"/> where
		/// each line in the string is limited to a number of characters equal or less than
		/// <paramref name="charsPerLine"/>.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="charsPerLine"></param>
		/// <returns></returns>
		public static string GetMultilineString(string message, int charsPerLine)
		{
			_stringBuilder.Clear();
			for (var i = 0; i < message.Length; i += charsPerLine)
			{
				var strLength = charsPerLine;
				if (i + strLength > message.Length)
				{
					strLength = message.Length - i;
				}

				_stringBuilder.AppendLine(message.Substring(i, strLength));
			}

			return _stringBuilder.ToString();
		}

		/// <summary>
		/// Returns the number of characters that can be made visible in a <see cref="Rect"/>
		/// <paramref name="rect"/> based on its width and the length of <see cref="string"/>
		/// <paramref name="message"/>.
		/// </summary>
		/// <param name="rect">The rect that will be used to display <paramref name="message"/></param>
		/// <param name="message">The message to be displayed.</param>
		/// <returns></returns>
		public static int GetCharactersPerRow(Rect rect, string message)
		{
			var messageChunkSize = 1f;
			var messageWidth = GUI.skin.label.CalcSize(new GUIContent(message));
			if (messageWidth.x > rect.width)
			{
				messageChunkSize = rect.width / messageWidth.x;
			}

			return Mathf.CeilToInt(messageChunkSize * message.Length);
		}
	}
}

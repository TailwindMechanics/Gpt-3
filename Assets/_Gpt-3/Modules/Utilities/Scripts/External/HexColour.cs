using UnityEngine;
using System;


namespace Modules.Utilities.External
{
	[Serializable]
	public struct HexColor
	{
		public string Hex => hexCode.Contains("#") ? hexCode : $"#{hexCode}";

		[SerializeField] string hexCode;

		public Color Color
		{
			get
			{
				ColorUtility.TryParseHtmlString(hexCode, out Color color);
				return color;
			}
		}

		public HexColor(Color color)
		{
			hexCode = ColorUtility.ToHtmlStringRGB(color);
		}

		public HexColor(string hexCode)
		{
			this.hexCode = hexCode;
		}
	}
}
using Modules.Utilities.External;
using UnityEngine;
using System;


namespace Modules.SyntaxHighlighter.Internal.DataObjects
{
	[Serializable]
	public class HighlightSettingsVo
	{
		public string KeywordColor => keywordColor.Hex;
		public string CommentColor => commentColor.Hex;
		public string StringColor => stringColor.Hex;
		public string PropertyNameColor => propertyNameColor.Hex;
		public string NumberColor => numberColor.Hex;
		public string BooleanOrNullColor => booleanOrNullColor.Hex;

		[SerializeField] HexColor keywordColor = new("#569CD6");
		[SerializeField] HexColor commentColor = new("#608B4E");
		[SerializeField] HexColor stringColor = new("#D69D85");
		[SerializeField] HexColor propertyNameColor = new("#9CDCFE");
		[SerializeField] HexColor numberColor = new("#B5CEA8");
		[SerializeField] HexColor booleanOrNullColor = new("#D7BA7D");
	}
}
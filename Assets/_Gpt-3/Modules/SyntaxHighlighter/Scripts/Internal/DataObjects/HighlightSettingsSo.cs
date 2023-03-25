using Sirenix.OdinInspector;
using UnityEngine;


namespace Modules.SyntaxHighlighter.Internal.DataObjects
{
	[CreateAssetMenu(fileName = "new _highlightSettings", menuName = "Tailwind/SyntaxHighlighter/Settings")]
	public class HighlightSettingsSo : ScriptableObject
	{
		public HighlightSettingsVo Vo => settings;
		[HideLabel, SerializeField] HighlightSettingsVo settings;
	}
}
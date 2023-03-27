using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.So
{
	[CreateAssetMenu(fileName = "new _webSearchSettings", menuName = "Tailwind/Web Search/Settings")]
	public class WebSearchSettingsSo : ScriptableObject
	{
		public WebSearchSettingsVo Vo => settings;
		[HideLabel, SerializeField] WebSearchSettingsVo settings;
	}
}
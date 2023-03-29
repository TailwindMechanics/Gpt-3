using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.So
{
	[CreateAssetMenu(fileName = "new _openAiSettings", menuName = "Tailwind/OpenAi/Settings")]
	public class OpenAiSettingsSo : ScriptableObject
	{
		public OpenAiSettingsVo Vo => settings;
		[HideLabel, SerializeField] OpenAiSettingsVo settings;
	}
}
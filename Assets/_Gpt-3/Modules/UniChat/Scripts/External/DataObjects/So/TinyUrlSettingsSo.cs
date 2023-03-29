using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.So
{
	[CreateAssetMenu(fileName = "new _tinyUrlSettings", menuName = "Tailwind/TinyUrl/Settings")]
	public class TinyUrlSettingsSo : ScriptableObject
	{
		public TinyUrlSettingsVo Vo => settings;
		[HideLabel, SerializeField] TinyUrlSettingsVo settings;
	}
}
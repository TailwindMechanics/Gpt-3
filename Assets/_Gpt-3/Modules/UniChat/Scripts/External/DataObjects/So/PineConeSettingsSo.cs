using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.So
{
	[CreateAssetMenu(fileName = "new _pineConeSettings", menuName = "Tailwind/PineCone/Settings")]
	public class PineConeSettingsSo : ScriptableObject
	{
		public PineConeSettingsVo Vo => settings;
		[HideLabel, SerializeField] PineConeSettingsVo settings;
	}
}
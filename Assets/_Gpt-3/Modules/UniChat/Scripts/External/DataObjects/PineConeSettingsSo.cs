using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.UniChat.External.DataObjects
{
	[CreateAssetMenu(fileName = "new _pineConeSettings", menuName = "Tailwind/PineCone/Settings")]
	public class PineConeSettingsSo : ScriptableObject
	{
		public PineConeSettingsVo Vo => settings;
		[HideLabel, SerializeField] PineConeSettingsVo settings;
	}
}
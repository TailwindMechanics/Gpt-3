using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.So
{
	[CreateAssetMenu(fileName = "new _watsonDiscoverySettings", menuName = "Tailwind/Watson/Discovery Settings")]
	public class WatsonDiscoverySettingsSo : ScriptableObject
	{
		public WatsonDiscoverySettingsVo Vo => settings;
		[SerializeField] WatsonDiscoverySettingsVo settings;
	}
}
using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.So
{
	[CreateAssetMenu(fileName = "new _googleCloudVisionSettings", menuName = "Tailwind/Google Cloud Vision/Settings")]
	public class GoogleCloudVisionSettingsSo : ScriptableObject
	{
		public GoogleCloudVisionSettingsVo Vo => settings;
		[HideLabel, SerializeField] GoogleCloudVisionSettingsVo settings;
	}
}
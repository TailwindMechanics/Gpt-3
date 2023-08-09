using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.So
{
	[CreateAssetMenu(fileName = "new _aiNavigationSettings", menuName = "Tailwind/Navigation/Settings")]
	public class AiNavigationSettingsSo : ScriptableObject
	{
		public AiNavigationSettingsVo Vo => settings;
		[HideLabel, SerializeField] AiNavigationSettingsVo settings;
	}
}
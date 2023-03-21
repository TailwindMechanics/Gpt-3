using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.So
{
	[CreateAssetMenu(fileName = "new _aiPerceptionSettings", menuName = "Tailwind/Perception/Settings")]
	public class AiPerceptionSettingsSo : ScriptableObject
	{
		public AiPerceptionSettingsVo Vo => settings;
		[HideLabel, SerializeField] AiPerceptionSettingsVo settings;
	}
}
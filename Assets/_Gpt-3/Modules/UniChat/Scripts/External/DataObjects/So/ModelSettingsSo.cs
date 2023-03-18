using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.So
{
	[CreateAssetMenu(fileName = "new _modelSettings", menuName = "Tailwind/OpenAi/Model Settings")]
	public class ModelSettingsSo : ScriptableObject
	{
		public ModelSettingsVo Vo => settings;
		[HideLabel, SerializeField] ModelSettingsVo settings;
	}
}
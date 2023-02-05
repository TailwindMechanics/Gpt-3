using Sirenix.OdinInspector;
using UnityEngine;


namespace Modules.UniChat.External.DataObjects
{
	[CreateAssetMenu(fileName = "new _promptTemplate", menuName = "Tailwind/Chat/Prompt Template")]
	public class PromptTemplateSo : ScriptableObject
	{
		public PromptTemplateVo Template => template;
		[HideLabel, SerializeField] PromptTemplateVo template;
	}
}
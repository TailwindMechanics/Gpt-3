using UnityEngine;


namespace Modules.UniChat.External.DataObjects
{
	public class PromptTemplateVo
	{
		public string Direction;
		public string Memory;
		public string UserMessage;
		public string UserName;

		public PromptTemplateVo AddDirection (string newDirection)
		{ Direction = newDirection; return this; }
		public PromptTemplateVo AddMessage (string newMessage)
		{ UserMessage = newMessage; return this; }
		public PromptTemplateVo AddMemory (string newMemory)
		{ Memory = newMemory; return this; }
		public PromptTemplateVo AddUsername (string newUsername)
		{ UserName = newUsername; return this; }
		public string Json () => JsonUtility.ToJson(this);
	}
}
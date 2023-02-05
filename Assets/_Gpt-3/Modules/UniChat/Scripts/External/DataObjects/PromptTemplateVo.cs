using UnityEngine;


namespace Modules.UniChat.External.DataObjects
{
	public class PromptTemplateVo
	{
		public string Direction;
		public string Memory;
		public string NewUserMessage;

		public PromptTemplateVo AddDirection (string newDirection)
		{ Direction = newDirection; return this; }
		public PromptTemplateVo AddMessage (string newMessage)
		{ NewUserMessage = newMessage; return this; }
		public PromptTemplateVo AddMemory (string newMemory)
		{ Memory = newMemory; return this; }
		public string Json () => JsonUtility.ToJson(this);
	}
}
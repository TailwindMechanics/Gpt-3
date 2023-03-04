using Newtonsoft.Json;
using UnityEngine;


namespace Modules.UniChat.External.DataObjects
{
	public class PromptTemplateVo
	{
		[JsonProperty] public string System;
		[JsonProperty] public string UserMessage;
		[JsonProperty] public string UserName;

		public PromptTemplateVo AddSystem (string system)
		{ System = system; return this; }
		public PromptTemplateVo AddMessage (string userMessage)
		{ UserMessage = userMessage; return this; }
		public PromptTemplateVo AddUsername (string username)
		{ UserName = username; return this; }
		public string Json () => JsonUtility.ToJson(this);
	}
}
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI.Chat;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.Interfaces
{
	public interface IChatBotApi
	{
		Task<AgentReply> GetReply(string senderName, string senderMessage, AiPerceivedData sightData, string direction, ModelSettingsVo settings, List<MessageVo> context, List<MessageVo> history, List<Function> functions, bool logging = false);
	}
}
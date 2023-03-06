using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI.Models;


namespace Modules.UniChat.External.DataObjects
{
	public interface IChatBotApi
	{
		Task<string> GetEmbedding(string message, Model model);
		Task<string> GetTextReply(string messageText, ChatBotSettingsVo settings);
		Task<(string response, List<float> embedding)> GetChatReply(string system, HistoryVo chatHistory, Model embeddingModel);
	}
}
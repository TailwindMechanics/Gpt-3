using System.Threading.Tasks;


namespace Modules.UniChat.External.DataObjects
{
	public interface IChatBotApi
	{
		Task<string> GetTextReply(string messageText, ChatBotSettingsVo settings);
		Task<string> GetChatReply(string system, HistoryVo chatHistory);
	}
}
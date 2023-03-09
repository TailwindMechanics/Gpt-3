using System.Collections.Generic;
using System.Threading.Tasks;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.Interfaces
{
	public interface IChatBotApi
	{
		Task<string> GetReply(string senderMessage, string direction, List<MessageVo> context, List<MessageVo> history, bool logging = false);
	}
}
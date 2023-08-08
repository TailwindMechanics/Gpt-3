using System.Threading.Tasks;
using OpenAI.Chat;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.Interfaces
{
	public interface IStructuredChatApi
	{
		Task<T> GetStructuredReply<T>(string prompt, string context, Function replySchema, ModelSettingsVo settings, bool logging = false);
	}
}
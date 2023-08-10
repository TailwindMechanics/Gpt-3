using OpenAI.Chat;


namespace Modules.UniChat.External.DataObjects.Vo
{
	public class AgentReply
	{
		public Message Message { get; }
		public Function Function { get; }

		public AgentReply(Message message, Function function)
		{
			Message = message;
			Function = function;
		}
	}
}
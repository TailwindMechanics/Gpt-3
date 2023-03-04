using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI.Completions;
using OpenAI.Chat;
using OpenAI;


namespace Modules.UniChat.External.DataObjects
{
	public class ChatBotApi : IChatBotApi
	{
		readonly OpenAIClient api;


		public ChatBotApi()
			=> api = new OpenAIClient();

		public async Task<string> GetChatReply(string system, HistoryVo chatHistory)
		{
			var chatPrompts = new List<ChatPrompt> {new("system", system)};

			chatHistory.Data.ForEach(item =>
			{
				var key = item.IsBot ? "assistant" : "user";
				chatPrompts.Add(new ChatPrompt(key, item.Message));
			});

			var chatRequest = new ChatRequest(chatPrompts);
			var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);

			return result.FirstChoice.ToString();
		}

		public async Task<string> GetTextReply(string messageText, ChatBotSettingsVo settings)
		{
			var request = new CompletionRequest
			(
				prompt: messageText + "\n",
				maxTokens: settings.MaxTokens,
				temperature: settings.Temperature,
				presencePenalty: settings.PresencePenalty,
				frequencyPenalty: settings.FrequencyPenalty,
				model: settings.Model
			);

			var message = "";
			await foreach (var token in api.CompletionsEndpoint.StreamCompletionEnumerableAsync(request))
			{
				var tokenString = token.ToString();
				message += tokenString;
			}

			return message;
		}
	}
}
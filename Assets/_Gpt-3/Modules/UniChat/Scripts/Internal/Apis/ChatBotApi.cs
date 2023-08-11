using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using UnityEngine;
using OpenAI.Chat;
using OpenAI;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.Apis
{
	public class ChatBotApi : IChatBotApi
	{
		readonly OpenAIClient openAiApi = new();

		public async Task<AgentReply> GetReply(string senderName, string senderMessage, string direction, ModelSettingsVo settings,
			List<MessageVo> context, List<MessageVo> history, List<Function> functions, bool logging = false)
		{
			try
			{
				var chatPrompts = new List<Message>();

				context.ForEach(item =>
				{
					var key = item.IsBot ? Role.Assistant : Role.User;
					chatPrompts.Add(new Message(key, item.Message, $"MEMORY-{item.SenderName}"));
				});
				history.ForEach(item =>
				{
					var key = item.IsBot ? Role.Assistant : Role.User;
					chatPrompts.Add(new Message(key, item.Message, $"HISTORY-{item.SenderName}"));
				});

				chatPrompts.Add(new Message(Role.User, senderMessage, $"NEW-{senderName}"));
				chatPrompts.Add(new Message(Role.System, direction, "AGENT-Direction"));

				var chatRequest = new ChatRequest
				(
					messages: chatPrompts,
					model: settings.Model,
					temperature: settings.Temperature,
					topP: settings.TopP,
					maxTokens: settings.MaxTokens,
					presencePenalty: settings.PresencePenalty,
					frequencyPenalty: settings.FrequencyPenalty,
					functions: functions,
					functionCall: "auto"
				);

				if (logging)
				{
					Log($"Sending chatRequest: {JsonConvert.SerializeObject(chatRequest)}");
				}

				var response = await openAiApi.ChatEndpoint.GetCompletionAsync(chatRequest);

				if (logging)
				{
					Log($"Received chat response: {JsonConvert.SerializeObject(response)}");
				}

				return new AgentReply(
					response.FirstChoice.Message,
					response.FirstChoice.Message.Function
				);
			}
			catch (HttpRequestException ex)
			{
				Debug.LogError($"OpenAI error: {ex.Message}");
				throw;
			}
		}


		void Log(string message)
			=> Debug.Log($"<color=#ADD9D9><b>>>> ChatBotApi: {message.Replace("\n", "")}</b></color>");
	}
}
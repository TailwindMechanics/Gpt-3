using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using OpenAI.Chat;
using UnityEngine;
using OpenAI;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.Vo;
using Newtonsoft.Json.Linq;

namespace Modules.UniChat.Internal.Apis
{
	public class StructuredChatApi : IStructuredChatApi
	{
		readonly OpenAIClient openAiApi = new();

		public async Task<T> GetStructuredReply<T>(string prompt, string context, Function replySchema, ModelSettingsVo settings, bool logging = false)
		{
			try
			{
				var messages = new List<Message>
				{
					new (Role.User, prompt, "sceneData"),
					new (Role.System, settings.Direction, "direction"),
					new (Role.System, context, "context")
				};

				var functions = new List<Function> { replySchema };
				var chatRequest = new ChatRequest(messages, functions: functions, functionCall: replySchema.Name, model: settings.Model);

				if (logging)
				{
					Log($"Sending structured chatRequest: {JsonConvert.SerializeObject(chatRequest)}");
				}

				var chatResponse = await openAiApi.ChatEndpoint.GetCompletionAsync(chatRequest);

				if (logging)
				{
					Log($"Received structured chat response: {JsonConvert.SerializeObject(chatResponse)}");
				}

				var arguments = chatResponse.FirstChoice.Message.Function.Arguments;
				if (arguments == null)
				{
					throw new JsonException("Expected properties not found in chat response.");
				}

				return JObject.Parse(arguments.ToString()).ToObject<T>();
			}
			catch (HttpRequestException ex)
			{
				Debug.LogError($"OpenAI error in StructuredChatApi: {ex.Message}");
				throw;
			}
		}

		void Log(string message)
			=> Debug.Log($"<color=#5ead8f><b>>>> StructuredChatApi: {message.Replace("\n", "")}</b></color>");
	}
}
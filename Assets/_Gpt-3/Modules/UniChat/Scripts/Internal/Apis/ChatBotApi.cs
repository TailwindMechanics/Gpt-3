using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using OpenAI.Chat;
using UnityEngine;
using OpenAI;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.Apis
{
    public class ChatBotApi : IChatBotApi
    {
        readonly OpenAIClient openAiApi;

        public ChatBotApi()
        {
            try
            {
                openAiApi = new OpenAIClient();
            }
            catch (HttpRequestException ex)
            {
                Debug.LogError($"OpenAI error: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GetReply(string senderMessage, string direction, List<MessageVo> context, List<MessageVo> history, bool logging = false)
        {
            try
            {
                var chatPrompts = new List<ChatPrompt> {new("system", direction)};

                context.ForEach(item =>
                {
                    var key = item.IsBot ? "assistant" : "user";
                    chatPrompts.Add(new ChatPrompt(key, item.Message));
                });
                history.ForEach(item =>
                {
                    var key = item.IsBot ? "assistant" : "user";
                    chatPrompts.Add(new ChatPrompt(key, item.Message));
                });
                chatPrompts.Add(new ChatPrompt("user", senderMessage));

                var chatRequest = new ChatRequest(chatPrompts);

                if (logging)
                {
                    Log($"Sending chatRequest: {JsonConvert.SerializeObject(chatRequest)}");
                }

                var result = await openAiApi.ChatEndpoint.GetCompletionAsync(chatRequest);

                if (logging)
                {
                    Log($"Received chat response: {result.FirstChoice}");
                }

                return result.FirstChoice.ToString();
            }
            catch (HttpRequestException ex)
            {
                Debug.LogError($"OpenAI error: {ex.Message}");
                throw;
            }
        }

        void Log(string message)
            =>Debug.Log($"<color=#ADD9D9><b>>>> ChatBotApi: {message.Replace("\n", "")}</b></color>");
    }
}
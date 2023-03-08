using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using OpenAI.Chat;
using UnityEngine;
using OpenAI;

using Modules.UniChat.External.DataObjects.Interfaces.New;
using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.Behaviours
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
                var contextJson = JsonUtility.ToJson(context);
                var historyJson = JsonUtility.ToJson(history);
                var chatPrompts = new List<ChatPrompt>
                {
                    new("system", direction),
                    new("system", contextJson),
                };

                if (logging)
                {
                    Log($"Sending senderMessage: {senderMessage}");
                    Log($"Sending direction: {direction}");
                    Log($"Sending context: {contextJson}");
                    Log($"Sending history {historyJson}");
                }

                history.ForEach(item =>
                {
                    var key = item.IsBot ? "assistant" : "user";
                    chatPrompts.Add(new ChatPrompt(key, item.Message));
                });
                chatPrompts.Add(new ChatPrompt("user", senderMessage));

                var chatRequest = new ChatRequest(chatPrompts);
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
            =>Debug.Log($"<color=#b7d8d8><b>>>> ChatBotApi: {message}</b></color>");
    }
}
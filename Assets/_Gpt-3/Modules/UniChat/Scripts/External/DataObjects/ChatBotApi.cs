using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI.Completions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI.Models;
using OpenAI.Chat;
using OpenAI;


namespace Modules.UniChat.External.DataObjects
{
    public class ChatBotApi : IChatBotApi
    {
        readonly OpenAIClient openAiApi;


        public ChatBotApi()
            => openAiApi = new OpenAIClient();

        public async Task<(string response, List<float> embedding)> GetChatReply(string system, HistoryVo chatHistory, Model embeddingModel)
        {
            var chatPrompts = new List<ChatPrompt> { new("system", system) };

            chatHistory.Data.ForEach(item =>
            {
                var key = item.IsBot ? "assistant" : "user";
                chatPrompts.Add(new ChatPrompt(key, item.Message));
            });

            var chatRequest = new ChatRequest(chatPrompts);
            var result = await openAiApi.ChatEndpoint.GetCompletionAsync(chatRequest);

            var responseText = result.FirstChoice.ToString();
            var json = await GetEmbedding(responseText, embeddingModel);

            var jsonObject = JObject.Parse(json);
            var embedding = JArray.FromObject(jsonObject["data"])
                .First["embedding"]
                .ToObject<List<float>>();

            return (responseText, embedding);
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
            await foreach (var token in openAiApi.CompletionsEndpoint.StreamCompletionEnumerableAsync(request))
            {
                var tokenString = token.ToString();
                message += tokenString;
            }

            return message;
        }

        public async Task<string> GetEmbedding(string message, Model model)
        {
            var embeddingsResponse = await openAiApi.EmbeddingsEndpoint.CreateEmbeddingAsync(message, model);
            return JsonConvert.SerializeObject(embeddingsResponse);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OpenAI.Models;
using UnityEngine;
using System.Linq;
using System;


namespace Modules.UniChat.External.DataObjects
{
    public class ConversationHistoryManager : IConversationHistoryManager
    {
        readonly IPineConeApi pineConeApi;
        readonly IChatBotApi chatBotApi;
        readonly Model embeddingModel;

        public ConversationHistoryManager(Model newEmbeddingModel, IChatBotApi newChatBotApi, PineConeSettingsVo pineConeSettings)
        {
            embeddingModel = newEmbeddingModel;
            chatBotApi = newChatBotApi;
            pineConeApi = new PineConeApi(pineConeSettings);
        }

        public async Task<List<List<float>>> RetrieveConversationHistoryAsync(string userMessage, HistoryVo chatHistory, int nearestNeighbourQueryCount = 5)
        {
            // Get the embeddings for the current user message and the previous two AI messages
            var json = await chatBotApi.GetEmbedding(userMessage, embeddingModel);
            var jsonObject = JObject.Parse(json);
            Debug.Log(jsonObject);
            var dataArray = JArray.FromObject(jsonObject["data"]);
            Debug.Log(dataArray);
            var embeds = dataArray.First["embedding"];
            Debug.Log(embeds);
            var currentMessageEmbedding = embeds.ToObject<List<float>>();
            chatHistory.Data[^1].SetEmbedding(currentMessageEmbedding);
            var previousMessagesEmbeddings = new List<List<float>>();

            for (var i = 0; i < Math.Min(3, chatHistory.Data.Count); i++)
            {
                var message = chatHistory.Data[chatHistory.Data.Count - i - 1];
                previousMessagesEmbeddings.Add(message.Embedding.EmbeddingVector);
            }

            // Concatenate the embeddings
            Debug.Log($"<color=yellow><b>>>> previousMessagesEmbeddings: {previousMessagesEmbeddings}</b></color>");

            var conversationHistoryEmbedding = currentMessageEmbedding;
            if (previousMessagesEmbeddings.Count > 1)
            {
                conversationHistoryEmbedding = previousMessagesEmbeddings
                .SelectMany(x => x)
                .Concat(currentMessageEmbedding)
                .ToList();
            }

            // Store the conversation history embedding in PineCone for long-term storage
            var pineConeItem = new Dictionary<string, object>()
            {
                {"id", Guid.NewGuid().ToString()},
                {"embedding", conversationHistoryEmbedding}
            };

            await pineConeApi.AddItemsAsync(new List<Dictionary<string, object>> {pineConeItem});

            // Retrieve the nearest neighbor embeddings
            var query = new List<float>(conversationHistoryEmbedding);
            var pineConeResults = await pineConeApi.NearestNeighborsAsync(query, nearestNeighbourQueryCount);

            // Extract the embeddings from the PineCone results
            var conversationHistoryEmbeddings = new List<List<float>>();

            foreach (var result in pineConeResults)
            {
                if (result.TryGetValue("embedding", out var embedding) && embedding is List<object> list)
                {
                    var embeddingValues = list.Select(Convert.ToSingle).ToList();
                    conversationHistoryEmbeddings.Add(embeddingValues);
                }
            }

            return conversationHistoryEmbeddings;
        }
    }
}
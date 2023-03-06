using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenAI.Models;
using System.Linq;
using System;
using UnityEngine;


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
            var conversationHistoryEmbedding = await RetrieveEmbeddingAsync(userMessage, chatHistory);

            await StoreConversationHistoryEmbeddingAsync(conversationHistoryEmbedding);

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

        async Task<List<float>> RetrieveEmbeddingAsync(string userMessage, HistoryVo chatHistory)
        {
            // Get the embeddings for the current user message and the previous two AI messages
            var json = await chatBotApi.GetEmbedding(userMessage, embeddingModel);
            var embeddingResponse = JsonConvert.DeserializeObject<EmbeddingResponse>(json);
            var currentMessageEmbedding = embeddingResponse.Embeddings;

            chatHistory.Data[^1].SetEmbedding(currentMessageEmbedding);
            if (chatHistory.Data.Count <= 1) return currentMessageEmbedding;

            var previousMessagesEmbeddings = new List<List<float>>();
            for (var i = 0; i < Math.Min(3, chatHistory.Data.Count - 1); i++)
            {
                var message = chatHistory.Data[chatHistory.Data.Count - i - 2];
                previousMessagesEmbeddings.Add(message.Embedding.EmbeddingVector);
            }

            previousMessagesEmbeddings.Add(currentMessageEmbedding);
            return previousMessagesEmbeddings.SelectMany(x => x).ToList();
        }

        async Task StoreConversationHistoryEmbeddingAsync(List<float> conversationHistoryEmbedding)
        {
            Debug.Log("Embedding dimension: " + conversationHistoryEmbedding.Count);

            var pineConeItem = new Dictionary<string, object>()
            {
                {"id", Guid.NewGuid().ToString()},
                {"embedding", conversationHistoryEmbedding}
            };

            Debug.Log("PineCone item: " + JsonConvert.SerializeObject(pineConeItem));

            var response = await pineConeApi.AddItemsAsync(new List<Dictionary<string, object>> { pineConeItem });
            Debug.Log("PineCone response: " + JsonConvert.SerializeObject(response));
        }

        public class EmbeddingResponseData
        {
            [JsonProperty("embedding")]
            public List<float> Embedding { get; set; }

            [JsonProperty("index")]
            public int Index { get; set; }

            [JsonProperty("object")]
            public string Object { get; set; }
        }

        public class EmbeddingResponse
        {
            public List<float> Embeddings => Data[0].Embedding;

            [JsonProperty("data")]
            public List<EmbeddingResponseData> Data { get; set; }

            [JsonProperty("model")]
            public string Model { get; set; }

            [JsonProperty("object")]
            public string Object { get; set; }

            [JsonProperty("usage")]
            public Dictionary<string, int> Usage { get; set; }
        }
    }
}
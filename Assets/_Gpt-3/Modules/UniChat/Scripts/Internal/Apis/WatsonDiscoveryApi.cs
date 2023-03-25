using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using UnityEngine;
using System;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.Vo;

namespace Modules.UniChat.Internal.Apis
{
    public class WatsonDiscoveryApi : IWatsonDiscoveryApi
    {
        readonly WatsonDiscoverySettingsVo settings;

        public WatsonDiscoveryApi(WatsonDiscoverySettingsVo settingsVo)
            => settings = settingsVo;

        public async Task<WatsonDiscoveryResponse> Search(string query, bool logging = false)
        {
            try
            {
                if (logging)
                {
                    Log($"Searching: {query}");
                }

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.ApiKey}");

                var requestUrl = $"{settings.BaseUrl}/v1/environments/{settings.EnvironmentId}/collections/{settings.CollectionId}/query?version={settings.Version}&query={Uri.EscapeDataString(query)}";
                var response = await httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonConvert.DeserializeObject<WatsonDiscoveryResponse>(responseContent);

                if (logging)
                {
                    Log($"Received Watson Discovery results: {responseContent.Length}");
                }

                return searchResponse;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error searching with Watson Discovery API: {ex.Message}");
                throw;
            }
        }

        void Log(string message)
            => Debug.Log($"<color=#a6D8cA><b>>>> WatsonDiscoveryApi: {message.Replace("\n", "")}</b></color>");
    }
}
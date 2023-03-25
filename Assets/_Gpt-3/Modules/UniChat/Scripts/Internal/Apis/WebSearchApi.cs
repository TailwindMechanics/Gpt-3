using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using UnityEngine;
using System;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.Apis
{
	public class WebSearchApi : IGoogleSearchApi
	{
		readonly WebSearchSettingsVo settings;


		public WebSearchApi(WebSearchSettingsVo settingsVo)
			=> settings = settingsVo;

		public async Task<GoogleSearchResponse> Search(string query, bool logging = false)
		{
			try
			{
				if (logging)
				{
					Log($"Searching: {query}");
				}

				var httpClient = new HttpClient();
				var requestUrl = $"{settings.BaseUrl}{settings.ApiKey}&cx={settings.EngineId}&q={Uri.EscapeDataString(query)}";
				var response = await httpClient.GetAsync(requestUrl);

				response.EnsureSuccessStatusCode();

				var responseContent = await response.Content.ReadAsStringAsync();
				var searchResponse = JsonConvert.DeserializeObject<GoogleSearchResponse>(responseContent);

				if (logging)
				{
					Log($"Received Google Search results: {responseContent.Length}");
				}

				return searchResponse;
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error searching with Google API: {ex.Message}");
				throw;
			}
		}

		void Log(string message)
			=> Debug.Log($"<color=#a6D8cA><b>>>> GoogleSearchApi: {message.Replace("\n", "")}</b></color>");
	}
}
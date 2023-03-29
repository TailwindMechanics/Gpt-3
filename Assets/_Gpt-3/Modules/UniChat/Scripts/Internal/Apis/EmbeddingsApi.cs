using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using OpenAI.Models;
using UnityEngine;
using OpenAI;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.Apis
{
	public class EmbeddingsApi : IEmbeddingsApi
	{
		readonly OpenAIClient openAiApi;


		public EmbeddingsApi(OpenAiSettingsVo settings)
			=> openAiApi = new OpenAIClient(new OpenAIAuthentication(settings.ApiKey, settings.OrgId));

		public async Task<IReadOnlyList<double>> ConvertToVector(Model model, string sender, string message, bool logging = false)
		{
			if (logging)
			{
				Log($"Converting message '{message}' to embedding vector...");
			}

			try
			{
				var embeddingsResponse = await openAiApi.EmbeddingsEndpoint.CreateEmbeddingAsync(message, model);
				var embedding = embeddingsResponse.Data[0].Embedding;

				if (logging)
				{
					Log($"Message '{message}' converted to embedding vector with {embedding.Count} dimensions");
				}

				return embedding;
			}
			catch (HttpRequestException ex)
			{
				Debug.LogError($"OpenAI API error: {ex.Message}");
				throw;
			}
		}

		void Log (string message)
			=> Debug.Log($"<color=#E0E08D><b>>>> EmbeddingsApi: {message.Replace("\n", "")}</b></color>");
	}
}
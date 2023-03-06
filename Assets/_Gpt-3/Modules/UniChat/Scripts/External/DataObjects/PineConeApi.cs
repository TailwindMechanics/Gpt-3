using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects
{
	public class PineConeApi : IPineConeApi
	{
		readonly PineConeSettingsVo settings;
		readonly HttpClient httpClient;


		[Serializable]
		public class PineConeVector
		{
			[JsonProperty("id")]
			public string Id { get; set; }

			[JsonProperty("metadata")]
			public Dictionary<string, object> Metadata { get; set; }

			[JsonProperty("values")]
			public List<double> Values { get; set; }
		}

		[Serializable]
		public class PineConeUpsertPayload
		{
			[JsonProperty("vectors")]
			public List<PineConeVector> Vectors { get; set; }

			[JsonProperty("namespace")]
			public string Namespace { get; set; }
		}

		[Serializable]
		public class PineConeQuery
		{
			[JsonProperty("vector")]
			public List<float> Vector { get; set; }

			[JsonProperty("topK")]
			public int TopK { get; set; }

			[JsonProperty("includeMetadata")]
			public bool IncludeMetadata { get; set; }

			[JsonProperty("includeValues")]
			public bool IncludeValues { get; set; }

			[JsonProperty("namespace")]
			public string Namespace { get; set; }
		}

		public PineConeApi(PineConeSettingsVo newSettings)
		{
			settings = newSettings;
			httpClient = new HttpClient();
		}

		public async Task<string> AddItemsAsync(List<Dictionary<string, object>> items)
		{
			Debug.Log($"<color=yellow><b>>>> .: {items.Count}</b></color>");
			var vectors = items.Select(item =>
				new PineConeVector
				{
					Id = item["id"].ToString(),
					Metadata = item,
					Values = item["embedding"] as List<double>
				}).ToList();

			var payload = new PineConeUpsertPayload
			{
				Vectors = vectors,
				Namespace = ""
			};

			var json = JsonConvert.SerializeObject(payload);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			var request = new HttpRequestMessage(HttpMethod.Post, settings.UpsertEndpoint);
			request.Content = content;
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			request.Headers.Add("Api-Key", $"{settings.ApiKey}");
			var response = await httpClient.SendAsync(request);

			try
			{
				response.EnsureSuccessStatusCode();
			}
			catch (HttpRequestException ex)
			{
				var exceptionResponse = await response.Content.ReadAsStringAsync();
				Debug.LogError($"AddItemsAsync failed with error: {ex.Message}, Response content: {exceptionResponse}");
				throw;
			}

			var responseContent = await response.Content.ReadAsStringAsync();
			return responseContent;
		}

		public async Task<List<Dictionary<string, object>>> NearestNeighborsAsync(List<float> embedding, int numNeighbors)
		{
			var query = new List<float>(embedding.Select(Convert.ToSingle));
			var responseContent = await SearchAsync(query, numNeighbors);
			var responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

			// Check if the 'matches' key is present instead of the 'data' key
			var neighbors = new List<Dictionary<string, object>>();
			if (responseDict.TryGetValue("matches", out var matches) && matches is List<object> matchList)
			{
				foreach (var match in matchList)
				{
					if (match is Dictionary<string, object> matchDict)
					{
						neighbors.Add(matchDict);
					}
				}
			}
			return neighbors;
		}

		async Task<string> SearchAsync(List<float> query, int numNeighbors)
		{
			var pineConeQuery = new PineConeQuery
			{
				Vector = query,
				TopK = numNeighbors,
				IncludeMetadata = true,
				IncludeValues = true,
				Namespace = ""
			};

			var json = JsonConvert.SerializeObject(pineConeQuery);
			Debug.Log(json);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			Debug.Log(content);

			var request = new HttpRequestMessage(HttpMethod.Post, settings.QueryEndpointPath)
			{
				Content = content
			};
			request.Headers.Add("Api-Key", $"{settings.ApiKey}");

			Debug.Log(request);

			var response = await httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();
			var responseContent = await response.Content.ReadAsStringAsync();

			Debug.Log(responseContent);

			return responseContent;
		}
	}
}

/*

curl - X POST\
https: //uni-chat-long-term-memory-3e09ea9.svc.us-west1-gcp.pinecone.io/query \
	-H 'Content-Type: application/json'\ -
	H 'Api-Key: 8dff4d4a-72ee-457e-b397-e33c70357e53'\ -
	d '{
"vector": [0,0,0,0], // This has been redacted, it is 1536 dimensions
"topK": 5,
"includeMetadata": true,
"includeValues": true,
"namespace": ""
}
'


*/
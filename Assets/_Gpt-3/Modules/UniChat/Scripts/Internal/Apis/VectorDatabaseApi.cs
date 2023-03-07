using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Interfaces.New;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.Internal.DataObjects;


namespace Modules.UniChat.Internal.Apis
{
	public class VectorDatabaseApi : IVectorDatabaseApi
	{
		readonly PineConeSettingsVo settings;
		readonly HttpClient httpClient;


		public VectorDatabaseApi (PineConeSettingsVo settingsVo)
		{
			settings = settingsVo;
			httpClient = new HttpClient();
		}

		public async Task<string> Query(IReadOnlyList<double> vector, bool logging = false)
		{
			var responseContent = await SearchAsync(vector, settings.NumberOfNeighbours, logging);
			var json = JsonUtility.ToJson(responseContent);
			// Debug.Log(json);
			return json;
		}

		public Task<string> Upsert(string message)
		{
			throw new System.NotImplementedException();
		}

		async Task<PineConeResponseVo> SearchAsync(IEnumerable<double> vector, int numNeighbors, bool logging)
		{
			if (logging)
			{
				Log("Sending Pinecone search request...");
			}

			try
			{
				var pineConeQuery = new PineConeQueryVo
				{
					Vector = vector.ToArray(),
					TopK = numNeighbors,
					IncludeMetadata = true,
					IncludeValues = true,
					Namespace = ""
				};

				var json = JsonConvert.SerializeObject(pineConeQuery);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var request = new HttpRequestMessage(HttpMethod.Post, settings.QueryEndpointPath)
				{
					Content = content
				};

				request.Headers.Add("Api-Key", $"{settings.ApiKey}");
				var response = await httpClient.SendAsync(request);

				response.EnsureSuccessStatusCode();
				var responseContent = await response.Content.ReadAsStringAsync();

				var data = JsonConvert.DeserializeObject<PineConeResponseVo>(responseContent);

				if (logging)
				{
					Log($"Received Pinecone search response with {data.Matches.Length} matches");
				}

				return data;
			}
			catch (HttpRequestException ex)
			{
				Debug.LogError($"HTTP request error: {ex.Message}");
				throw;
			}
			catch (JsonException ex)
			{
				Debug.LogError($"JSON deserialization error: {ex.Message}");
				throw;
			}
		}

		void Log (string message) => Debug.Log($"<color=#B4FFFF><b>>>> VectorDatabaseApi: {message}</b></color>");
	}
}
/*
{
	"results": [],
	"matches": [
	{
		"id": "id",
		"score": 0,
		"values": [0, 0, 0, 0, ...redacted, there are 1536 dimensions
	}],
	"namespace": ""
}
*/






/* PineCone Query
		curl - X POST\
		https: //uni-chat-long-term-memory-3e09ea9.svc.us-west1-gcp.pinecone.io/query \
			-H 'Content-Type: application/json'\ -
			H 'Api-Key: 8dff4d4a-72ee-457e-b397-e33c70357e53'\ -
			d '{
		"vector": [0,0,0,0... // This has been redacted, it is 1536 dimensions
		"topK": 5,
		"includeMetadata": true,
		"includeValues": true,
		"namespace": ""
		}
'
PineCone Query*/


/* PineCone Upsert
		curl -X POST \
		    https://uni-chat-long-term-memory-3e09ea9.svc.us-west1-gcp.pinecone.io/vectors/upsert \
		    -H 'Content-Type: application/json' \
		    -H 'Api-Key: 8dff4d4a-72ee-457e-b397-e33c70357e53' \
		    -d'{
		  "vectors": [
		    {
		      "id": "id",
		      "metadata": {},
			  "values": [0,0,0,0... // This has been redacted, it is 1536 dimensions
		    }
		  ],
		  "namespace": ""
		}'
PineCone Upsert*/
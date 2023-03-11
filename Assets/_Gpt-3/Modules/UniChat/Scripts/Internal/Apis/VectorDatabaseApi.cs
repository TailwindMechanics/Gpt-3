using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using UnityEngine;
using System;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.Internal.DataObjects;


namespace Modules.UniChat.Internal.Apis
{
	public class VectorDatabaseApi : IVectorDatabaseApi
	{
		const string indexStats		= "/describe_index_stats";
		const string upsert			= "/vectors/upsert";
		const string deleteVectors	= "/vectors/delete";
		const string query			= "/query";
		const int numNeighbours		= 5;

		readonly PineConeSettingsVo settings;
		readonly HttpClient httpClient;

		string Url (string endPoint) => $"{settings.IndexUrl}{endPoint}";


		public VectorDatabaseApi (PineConeSettingsVo settingsVo)
		{
			settings = settingsVo;
			httpClient = new HttpClient();
		}

		public async Task<string> DescribeIndexStats(bool logging = false)
		{
			try
			{
				if (logging)
				{
					Log("Sending PineCone DescribeIndexStats request...");
				}

				var request = new HttpRequestMessage(HttpMethod.Get, Url(indexStats));
				request.Headers.Add("Api-Key", settings.ApiKey);

				var response = await httpClient.SendAsync(request);

				response.EnsureSuccessStatusCode();

				var responseContent = await response.Content.ReadAsStringAsync();

				if (logging)
				{
					Log($"Received PineCone DescribeIndexStats response: {responseContent}");
				}

				return responseContent;
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error describing index stats: {ex.Message}");
				throw;
			}
		}

		public async Task<List<Guid>> Query(string nameSpace, IEnumerable<double> vector, float minScore, bool logging = false)
		{
			var responseContent = await SearchAsync(nameSpace, vector, numNeighbours, logging);
			var result = new List<Guid>();

			foreach (var match in responseContent.Matches)
			{
				if (match.Score < minScore) continue;

				if (Guid.TryParse(match.Id, out var item))
				{
					Log($"Score: {match.Score}");
					result.Add(item);
				}
				else if (logging)
				{
					Log($"Failed to parse PineCone id: '{match.Id}'");
				}
			}

			return result;
		}

		public async Task<Guid> Upsert(string nameSpace, IEnumerable<double> vector, bool logging = false)
		{
			try
			{
				var id = Guid.NewGuid();
				var item = new VectorDatabaseUpsertItem
				{
					Id = id.ToString(),
					Values = vector.ToList()
				};
				var upsertRequest = new VectorDatabaseUpsertRequest
				{
					Vectors = new List<VectorDatabaseUpsertItem> { item },
					Namespace = nameSpace
				};

				var json = JsonConvert.SerializeObject(upsertRequest);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var request = new HttpRequestMessage(HttpMethod.Post, Url(upsert))
				{
					Content = content
				};

				request.Headers.Add("Api-Key", $"{settings.ApiKey}");
				var response = await httpClient.SendAsync(request);

				response.EnsureSuccessStatusCode();

				if (logging)
				{
					Log($"Upserted {upsertRequest.Vectors.Count} vectors to VectorDatabase in namespace: {nameSpace}");
				}

				return id;
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error upserting data to VectorDatabase: {ex.Message} in namespace: {nameSpace}");
				throw;
			}
		}

		public async Task DeleteAllVectorsInNamespace(string nameSpace, bool logging = false)
		{
			try
			{
				if (logging)
				{
					Log($"Sending PineCone Delete All in namespace request, namespace: {nameSpace}...");
				}

				var deleteAllRequest = new PineConeDeleteRequestVo
				{
					DeleteAll = true,
					Namespace = nameSpace
				};

				var json = JsonConvert.SerializeObject(deleteAllRequest);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var request = new HttpRequestMessage(HttpMethod.Post, Url(deleteVectors))
				{
					Content = content
				};

				request.Headers.Add("Api-Key", $"{settings.ApiKey}");
				var response = await httpClient.SendAsync(request);

				response.EnsureSuccessStatusCode();

				if (logging)
				{
					Log($"Delete All in namespace response, namespace: {nameSpace}, response: {response.StatusCode}");
				}
			}

			catch (Exception ex)
			{
				Debug.LogError($"Error deleting data from VectorDatabase: {ex.Message}, namespace: {nameSpace}");
				throw;
			}
		}

		async Task<PineConeResponseVo> SearchAsync(string nameSpace, IEnumerable<double> vector, int numNeighbors, bool logging)
		{
			if (logging)
			{
				Log($"Sending PineCone search request in namespace: {nameSpace}...");
			}

			try
			{
				var pineConeQuery = new PineConeQueryVo
				{
					Vector = vector.ToArray(),
					TopK = numNeighbors,
					IncludeMetadata = true,
					IncludeValues = true,
					Namespace = nameSpace
				};

				var json = JsonConvert.SerializeObject(pineConeQuery);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var request = new HttpRequestMessage(HttpMethod.Post, Url(query))
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
					Log($"Received PineCone search response with {data.Matches.Length} matches  in namespace: {nameSpace}");
				}

				return data;
			}
			catch (HttpRequestException ex)
			{
				Debug.LogError($"HTTP request error: {ex.Message} in namespace: {nameSpace}");
				throw;
			}
			catch (JsonException ex)
			{
				Debug.LogError($"JSON deserialization error: {ex.Message} in namespace: {nameSpace}");
				throw;
			}
		}

		void Log (string message)
			=> Debug.Log($"<color=#B7D8BA><b>>>> VectorDatabaseApi: {message.Replace("\n", "")}</b></color>");
	}
}
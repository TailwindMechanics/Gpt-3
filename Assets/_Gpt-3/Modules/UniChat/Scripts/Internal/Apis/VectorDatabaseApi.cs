﻿using System.Collections.Generic;
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
		readonly PineConeSettingsVo settings;
		readonly HttpClient httpClient;


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

				var request = new HttpRequestMessage(HttpMethod.Get, $"{settings.IndexEndpoint}");
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

		public async Task<List<Guid>> Query(IEnumerable<double> vector, bool logging = false)
		{
			var responseContent = await SearchAsync(vector, settings.NumberOfNeighbours, logging);
			var result = new List<Guid>();

			foreach (var match in responseContent.Matches)
			{
				if (Guid.TryParse(match.Id, out var item))
				{
					result.Add(item);
				}
				else if (logging)
				{
					Log($"Failed to parse PineCone id: '{match.Id}'");
				}
			}

			return result;
		}

		public async Task<Guid> Upsert(IEnumerable<double> vector, bool logging = false)
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
					Vectors = new List<VectorDatabaseUpsertItem> { item }
				};

				var json = JsonConvert.SerializeObject(upsertRequest);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var request = new HttpRequestMessage(HttpMethod.Post, settings.UpsertEndpoint)
				{
					Content = content
				};

				request.Headers.Add("Api-Key", $"{settings.ApiKey}");
				var response = await httpClient.SendAsync(request);

				response.EnsureSuccessStatusCode();
				var responseContent = await response.Content.ReadAsStringAsync();

				if (logging)
				{
					Log($"Upserted {upsertRequest.Vectors.Count} vectors to VectorDatabase");
				}

				return id;
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error upserting data to VectorDatabase: {ex.Message}");
				throw;
			}
		}

		public async Task DeleteAll(string nameSpace = null, bool logging = false)
		{
			try
			{
				if (logging)
				{
					Log($"Sending PineCone Delete All request{InNameSpace(nameSpace)}...");
				}

				var deleteAllRequest = new PineConeDeleteRequestVo
				{
					DeleteAll = true,
					Namespace = nameSpace
				};

				var json = JsonConvert.SerializeObject(deleteAllRequest);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var request = new HttpRequestMessage(HttpMethod.Post, settings.DeleteEndpoint)
				{
					Content = content
				};

				request.Headers.Add("Api-Key", $"{settings.ApiKey}");
				var response = await httpClient.SendAsync(request);

				response.EnsureSuccessStatusCode();
				var responseContent = await response.Content.ReadAsStringAsync();

				if (logging)
				{
					Log($"Delete all response{InNameSpace(nameSpace)}, response: {response.StatusCode}");
				}
			}

			catch (Exception ex)
			{
				Debug.LogError($"Error deleting data from VectorDatabase: {ex.Message}");
				throw;
			}
		}

		async Task<PineConeResponseVo> SearchAsync(IEnumerable<double> vector, int numNeighbors, bool logging)
		{
			if (logging)
			{
				Log("Sending PineCone search request...");
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
					Log($"Received PineCone search response with {data.Matches.Length} matches");
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

		string InNameSpace(string nameSpace)
			=> !string.IsNullOrWhiteSpace(nameSpace) ? $" in namespace: {nameSpace}" : "";

		void Log (string message)
			=> Debug.Log($"<color=#B7D8BA><b>>>> VectorDatabaseApi: {message.Replace("\n", "")}</b></color>");
	}
}
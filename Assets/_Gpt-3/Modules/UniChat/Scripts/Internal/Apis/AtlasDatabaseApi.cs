using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using UnityEngine;
using System;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.Apis
{
    public class AtlasDatabaseApi : IAtlasDatabaseApi
    {
        const string upsert = "/api/atlas/v1.0/groups/your_project_id/data-lake/your_cluster_name/db/Tailwind_db/collections/ShopTestScene_collection/documents";

        readonly AtlasSettingsVo settings;
        readonly HttpClient httpClient;

        string BaseUrl(string endPoint)
            => $"{settings.BaseUrl}{endPoint}";


        public AtlasDatabaseApi(AtlasSettingsVo settings)
        {
            this.settings = settings;
            httpClient = new HttpClient();
        }

        // public async Task UpsertChunk(ChunkData sceneChunk, bool logging = false)
        // {
        //     try
        //     {
        //         if (logging)
        //         {
        //             Log($"Sending Atlas UpsertChunk request, chunk: {sceneChunk.Position}...");
        //         }
        //
        //         var filter = new { chunk_position = sceneChunk.Position };
        //         var filterJson = JsonConvert.SerializeObject(filter);
        //         var updateJson = JsonConvert.SerializeObject(sceneChunk);
        //         var update = new { filter = filterJson, update = updateJson, upsert = true };
        //         var updateContent = JsonConvert.SerializeObject(update);
        //
        //         var content = new StringContent(updateContent, Encoding.UTF8, "application/json");
        //         var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl(upsert))
        //         {
        //             Content = content
        //         };
        //
        //         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);
        //
        //         var response = await httpClient.SendAsync(request);
        //         response.EnsureSuccessStatusCode();
        //
        //         if (logging)
        //         {
        //             Log($"Upserted SceneChunk in Atlas, chunk: {sceneChunk.Position}.");
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Debug.LogError($"Error upserting chunk: {sceneChunk.Position} to Atlas: {ex.Message}");
        //         throw;
        //     }
        // }

        void Log(string message)
            => Debug.Log($"<color=#B7D8BA><b>>>> AtlasDatabaseApi: {message.Replace("\n", "")}</b></color>");
    }
}
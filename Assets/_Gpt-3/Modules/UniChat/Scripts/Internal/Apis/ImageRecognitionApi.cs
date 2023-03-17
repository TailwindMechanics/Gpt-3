using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using UnityEngine;
using System.IO;
using System;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.So;
using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.Apis
{
	public class ImageRecognitionApi : IImageRecognitionApi
	{
        public async Task<List<LabelAnnotation>> AnalyzeImage(string imagePath, GoogleCloudVisionSettingsSo settings, bool logging = false)
        {
            try
            {
                if (logging)
                {
                    Log($"analysing: {imagePath}");
                }

                var httpClient = new HttpClient();
                var imageBytes = await File.ReadAllBytesAsync(imagePath);
                var base64Image = Convert.ToBase64String(imageBytes);

                var imageRequest = new GoogleCloudVisionRequestVo
                {
                    Requests = new List<ImageRequest>
                    {
                        new()
                        {
                            Image = new ImageContent { Content = base64Image },
                            Features = new List<Feature> { new() { Type = "LABEL_DETECTION" } }
                        }
                    }
                };

                var requestBody = JsonConvert.SerializeObject(imageRequest);

                var request = new HttpRequestMessage(HttpMethod.Post, settings.Vo.Url)
                {
                    Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
                };

                var response = await httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var visionResponse = JsonConvert.DeserializeObject<GoogleCloudVisionResponseVo>(responseContent);

                if (logging)
                {
                    Log($"Received Google Cloud Vision response: {responseContent}");
                }

                if (visionResponse.Responses.Count > 0 && visionResponse.Responses[0].LabelAnnotations != null)
                {
                    if (!logging) return visionResponse.Responses[0].LabelAnnotations;

                    foreach (var label in visionResponse.Responses[0].LabelAnnotations)
                    {
                        Log($"Description: {label.Description}, Score: {label.Score}");
                    }

                    return visionResponse.Responses[0].LabelAnnotations;
                }

                Debug.LogError("No labels found in the response.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error analyzing screenshot: {ex.Message}");
                throw;
            }

            return null;
        }

        void Log(string message)
            => Debug.Log($"<color=#B7D8BA><b>>>> ImageRecognition: {message.Replace("\n", "")}</b></color>");
	}
}
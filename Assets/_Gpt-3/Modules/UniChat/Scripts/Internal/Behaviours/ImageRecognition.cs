using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using UnityEngine;
using System.IO;
using System;

using Modules.UniChat.External.DataObjects.So;
using Modules.UniChat.Internal.DataObjects;


namespace Modules.UniChat.Internal.Behaviours
{
    public class ImageRecognition : MonoBehaviour
    {
        [FilePath, SerializeField]
        string imagePath;

        [PropertyOrder(2),SerializeField]
        List<LabelAnnotation> labels;

        [InlineEditor, SerializeField]
        GoogleCloudVisionSettingsSo settings;

        [Button(ButtonSizes.Medium)]
        async void LogTest()
            => labels = await AnalyzeScreenshotAsync(imagePath);

        readonly HttpClient httpClient = new();


        async Task<List<LabelAnnotation>> AnalyzeScreenshotAsync(string screenshotPath)
        {
            try
            {
                Log($"analysing: {screenshotPath}");

                var imageBytes = await File.ReadAllBytesAsync(screenshotPath);
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

                Log($"Received Google Cloud Vision response: {responseContent}");

                if (visionResponse.Responses.Count > 0 && visionResponse.Responses[0].LabelAnnotations != null)
                {
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
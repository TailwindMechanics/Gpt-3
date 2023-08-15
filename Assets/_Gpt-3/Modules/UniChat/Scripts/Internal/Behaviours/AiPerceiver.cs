using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using System;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.Internal.DepthPerceiver;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.External.DataObjects;
using Modules.UniChat.Internal.Apis;
using Modules.Utilities.External;


namespace Modules.UniChat.Internal.Behaviours
{
    [Serializable]
    public class AiPerceiver : IAiPerceiver
    {
        GoogleCloudVisionSettingsVo creds;

        public AiPerceiver(GoogleCloudVisionSettingsVo creds)
            => this.creds = creds;

        public async Task<AiPerceivedData> CaptureVision (Camera cam, AiPerceptionSettingsVo settings)
        {
            Log($"Capturing from camera: {cam.name}");

            var grid = new GridManager();
            var chunkData = new AiPerceivedData
            {
                // todo create service for querying in-game datetime
                Time            = DateUtils.CurrentTime(),
                Day             = DateUtils.CurrentDay(),
                Season          = DateUtils.CurrentSeason(),
                Year            = DateUtils.CurrentYear(),

                // todo create services to query world for below types of data
                // Weather         = "Cel:22/21/19, hum:2%, pre:65%, wind/11kmh, partly cloudy",
                // Sounds          = "Close: children playing, music small speaker, cutlery, plates, spatula on grill, cooking, jovial walla. Med: seagulls overhead, car park sounds. Far: main road. Distant: arcade sounds",
                // Smells          = "Strong: pizza, ketchup, onions, cooking. Med: exhaust, beer. Subtle: tarmac",
                // Geographic      = "I am 14km nnw of home. At 'Canesbury Market' strip mall 6th time this year.",
                // Feelings        = "Safe, hungry, satisfied, happy, content, calm, warm, relieved, wholesome",
                // Doings          = "Sitting, eating, laughing",

                AgentBodyRadius  = "1.0m",
                AgentWorldPosition = cam.transform.position.ToString("F2"),
                AgentWorldHeading = cam.transform.eulerAngles.y.NormalizeDegrees().ToString("F2"),
                SceneObjects = grid.GetSceneObjects(cam, settings)
            };

            var fileName = $"{chunkData.Time}-{chunkData.Day}-{chunkData.Season}-{chunkData.Year}-{cam.name}";
            fileName = fileName.Replace(" ", "_").Replace("/", "-").Replace(":", "-");
            var visionResponse = await GoogleCloudVisionAnalyse(cam, fileName, settings);
            chunkData.CloudVisionData = new CloudVisionData(visionResponse);

            Log($"Captured perception data: {JsonConvert.SerializeObject(chunkData)}");
            return chunkData;
        }

        async Task<GoogleCloudVisionResponseVo> GoogleCloudVisionAnalyse(Camera cam, string fileName, AiPerceptionSettingsVo settings)
        {
            var imagePath = await ImageCapture.Capture(cam, settings.SavePath, fileName, settings.Resolution);
            var api = new ImageRecognitionApi() as IImageRecognitionApi;
            return await api.AnalyzeImage(imagePath, creds, true);
        }

        void Log(string message)
            =>Debug.Log($"<color=#ADB9D3><b>>>> AiPerceiver: {message.Replace("\n", "")}</b></color>");
    }
}
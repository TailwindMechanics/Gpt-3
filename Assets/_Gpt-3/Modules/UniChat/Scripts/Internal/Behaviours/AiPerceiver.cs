using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.External.DataObjects;
using Modules.UniChat.Internal.Apis;
using Modules.Utilities.External;


namespace Modules.UniChat.Internal.Behaviours
{
    [Serializable]
    public class AiPerceiver : IAiPerceiver
    {
        public async Task<string> CaptureVision (Camera cam, Transform volume, AiPerceptionSettingsVo settings)
        {
            Log($"Capturing from camera: {cam.name}");

            var camTransform    = cam.transform;
            var camPos          = camTransform.position;
            var camRot          = camTransform.eulerAngles;
            var volumeScale     = volume.localScale;
            var volumePos       = volume.position;

            volume.gameObject.SetActive(false);
            var chunkData = new AiPerceivedData
            {
                Time            = DateUtils.CurrentTime(),
                Day             = DateUtils.CurrentDay(),
                Season          = DateUtils.CurrentSeason(),
                Year            = DateUtils.CurrentYear(),

                // Weather         = "Cel:22/21/19, hum:2%, pre:65%, wind/11kmh, partly cloudy",
                // Sounds          = "Close: children playing, music small speaker, cutlery, plates, spatula on grill, cooking, jovial walla. Med: seagulls overhead, car park sounds. Far: main road. Distant: arcade sounds",
                // Smells          = "Strong: pizza, ketchup, onions, cooking. Med: exhaust, beer. Subtle: tarmac",
                // Geographic      = "I am 14km nnw of home. At 'Canesbury Market' strip mall 6th time this year. Drove here with Davey in my Flatbed Truck. Local Events: Food truck jamboree 17/04/23, 13:00-17:00",
                // Feelings        = "Safe, hungry, satisfied, happy, content, calm, warm, tipsy, mild need to pee, relieved, love, wholesome",
                // Doings          = "Sitting, eating, chatting with Davey, drinking beer, farting, smoking, laughing",

                YourPosition    = $"({camPos.x:F1},{camPos.y:F1},{camPos.z:F1})",
                YourRotation    = $"({camRot.x:F0},{camRot.y:F0},{camRot.z:F0})",
                AreaContent     = CaptureChunkContent(camTransform, volumePos, volumeScale, settings)
            };

            volume.gameObject.SetActive(true);

            var fileName = $"{chunkData.YourPosition}";
            var visionResponse = await GoogleCloudVisionAnalyse(cam, fileName, settings);
            chunkData.CloudVisionData = new CloudVisionData(visionResponse);

            var tuple = JsonUtilities.SaveAsJsonFile(settings.SavePath, fileName, chunkData);

            Log($"Captured data saved at path: {tuple.filePath}");
            return tuple.json;
        }

        List<string> CaptureChunkContent(Transform cam, Vector3 chunkPos, Vector3 chunkSize, AiPerceptionSettingsVo settings)
        {
            var bounds = new Bounds(chunkPos, chunkSize);
            var allRenderers = Object.FindObjectsOfType<Renderer>();

            return allRenderers.Where(renderer => bounds.Intersects(renderer.bounds))
                .Where(renderer => !settings.BlackList.Contains(renderer.gameObject.name))
                .Select(renderer => new { renderer, rendTransform = renderer.transform })
                .Select(tuple => AreaObjectData(cam, tuple.renderer, settings))
                .ToList();
        }

        string AreaObjectData(Transform cam, Renderer renderer, AiPerceptionSettingsVo settings)
        {
            var objectName = renderer.name;
            settings.RemoveFromNames.ForEach(item =>
            {
                if (objectName.Contains(item))
                {
                    objectName = objectName.Replace(item, "");
                }
            });

            objectName      = StripUnityLabels(objectName);
            var bounds      = renderer.bounds;
            var direction   = $"{cam.position - bounds.center:F1}";
            var size        = $"{bounds.size:F1}";

            return $"{objectName}: dir:{direction}, size:{size}";
        }

        string StripUnityLabels (string input)
        {
            input = input.Replace("(Clone)", "");
            for (var i = 0; i < 10; i++)
            {
                input = input.Replace($"({i})", "");
                input = input.Replace($"_0{i}", "");
            }

            input = input.Trim().Replace(" ", "_");

            return input;
        }

        async Task<GoogleCloudVisionResponseVo> GoogleCloudVisionAnalyse(Camera cam, string fileName, AiPerceptionSettingsVo settings)
        {
            var imagePath = await ImageCapture.Capture(cam, settings.SavePath, fileName, settings.Resolution);
            var api = new ImageRecognitionApi() as IImageRecognitionApi;
            return await api.AnalyzeImage(imagePath, settings.CloudVisionCreds, true);
        }

        void Log(string message)
            =>Debug.Log($"<color=#ADB9D3><b>>>> AiPerceiver: {message.Replace("\n", "")}</b></color>");
    }
}
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.So;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.External.DataObjects;
using Modules.UniChat.Internal.Apis;
using Modules.Utilities.External;


namespace Modules.UniChat.Internal.Behaviours
{
    [CreateAssetMenu(fileName = "new _imageRecognizer", menuName = "Tailwind/Google Cloud Vision/Image Recognizer")]
    public class ImageRecognizer : ScriptableObject
    {
        [SerializeField] Vector2Int imageResolution;
        [FolderPath, SerializeField] string savePath;
        [InlineEditor, SerializeField] GoogleCloudVisionSettingsSo gcvSettings;
        [SerializeField] List<string> areaRemoveList = new();
        [SerializeField] List<string> areaBlackList = new();


        public async void Capture (Camera cam, Transform volumeCube)
        {
            Debug.Log("<color=cyan><b>>>> Start Capture</b></color>");

            var camTransform    = cam.transform;
            var camPos          = camTransform.position;
            var camRot          = camTransform.eulerAngles;
            var volumeScale     = volumeCube.localScale;
            var volumePos       = volumeCube.position;

            volumeCube.gameObject.SetActive(false);
            var chunkData = new CameraViewData
            {
                Time            = "14:12",
                Day             = "Sunday April 23rd",
                Season          = "Spring",
                Year            = "2023",
                Weather         = "Cel:22/21/19, hum:2%, pre:65%, wind/11kmh, partly cloudy",
                Sounds          = "Close: children playing, music small speaker, cutlery, plates, spatula on grill, cooking, jovial walla. Med: seagulls overhead, car park sounds. Far: main road. Distant: arcade sounds",
                Smells          = "Strong: pizza, ketchup, onions, cooking. Med: exhaust, beer. Subtle: tarmac",
                Geographic      = "I am 14km nnw of home. At 'Canesbury Market' strip mall 6th time this year. Drove here with Davey in my Flatbed Truck. Local Events: Food truck jamboree 17/04/23, 13:00-17:00",
                Feelings        = "Safe, hungry, satisfied, happy, content, calm, warm, tipsy, mild need to pee, relieved, love, wholesome",
                Doings          = "Sitting, eating, chatting with Davey, drinking beer, farting, smoking, laughing",
                YourPosition    = $"({camPos.x:F1},{camPos.y:F1},{camPos.z:F1})",
                YourRotation    = $"({camRot.x:F0},{camRot.y:F0},{camRot.z:F0})",
                AreaContent     = CaptureChunkContent(volumePos, volumeScale)
            };

            volumeCube.gameObject.SetActive(true);

            var fileName = $"{chunkData.YourPosition}";
            var visionResponse = await GoogleCloudVisionAnalyse(cam, fileName, gcvSettings);
            chunkData.CloudVisionData = new CloudVisionData(visionResponse);
            var filePath = JsonUtilities.SaveAsJsonFile(savePath, fileName, chunkData);

            DoEditorRefreshAndSelect(filePath);
            Debug.Log("<color=yellow><b>>>> End Capture</b></color>");
        }

        List<string> CaptureChunkContent(Vector3 chunkPos, Vector3 chunkSize)
        {
            var bounds = new Bounds(chunkPos, chunkSize);
            var allRenderers = FindObjectsOfType<Renderer>();

            return allRenderers.Where(renderer => bounds.Intersects(renderer.bounds))
                .Where(renderer => !areaBlackList.Contains(renderer.gameObject.name))
                .Select(renderer => new { renderer, rendTransform = renderer.transform })
                .Select(tuple => AreaObjectData(tuple.renderer))
                .ToList();
        }

        string AreaObjectData(Renderer renderer)
        {
            var objectName = renderer.name;
            areaRemoveList.ForEach(item =>
            {
                if (objectName.Contains(item))
                {
                    objectName = objectName.Replace(item, "");
                }
            });

            objectName = StripUnityLabels(objectName);

            // Retrieve object's transform
            var t = renderer.transform;
            var pos = t.position;
            var eul = t.eulerAngles;

            // Retrieve bounding box size in meters
            var bounds = renderer.bounds.size;

            // Generate a more concise string representation for the object
            objectName = $"{objectName}: ({pos.x:F1},{pos.y:F1},{pos.z:F1}), ({eul.x:F0},{eul.y:F0},{eul.z:F0}), ({bounds.x:F1},{bounds.y:F1},{bounds.z:F1})";

            return objectName;
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

        void DoEditorRefreshAndSelect (string path)
        {
            AssetDatabase.Refresh();
            var exportedAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
            EditorGUIUtility.PingObject(exportedAsset);
            Selection.activeObject = exportedAsset;
        }

        async Task<GoogleCloudVisionResponseVo> GoogleCloudVisionAnalyse(Camera cam, string fileName, GoogleCloudVisionSettingsSo settings)
        {
            var imagePath = await ImageCapture.Capture(cam, savePath, fileName, imageResolution);
            var api = new ImageRecognitionApi() as IImageRecognitionApi;
            return await api.AnalyzeImage(imagePath, settings, true);
        }
    }
}
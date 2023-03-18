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
        [SerializeField] List<string> blackList = new();


        public async void Capture (Camera cam, Transform volumeCube)
        {
            Debug.Log("<color=cyan><b>>>> Start Capture</b></color>");

            var camTransform    = cam.transform;
            var volumeScale     = volumeCube.localScale;
            var volumePos       = volumeCube.position;

            volumeCube.gameObject.SetActive(false);
            var chunkData = new CameraViewData
            {
                TimeStamp       = DateTime.Now.ToString("yyyy-MM-dd_HH-mm"),
                CameraPosition  = new Vector3Serializable(camTransform.position),
                CameraRotation  = new Vector3Serializable(camTransform.eulerAngles),
                // AreaSize        = new Vector3Serializable(volumeScale),
                AreaContent     = CaptureChunkContent(volumePos, volumeScale)
            };

            volumeCube.gameObject.SetActive(true);

            var fileName = $"{chunkData.TimeStamp}_{chunkData.CameraPosition}";
            var visionResponse = await GoogleCloudVisionAnalyse(cam, fileName, gcvSettings);
            chunkData.CloudVisionData = new CloudVisionData(visionResponse);
            var filePath = JsonUtilities.SaveAsJsonFile(savePath, fileName, chunkData);

            DoEditorRefreshAndSelect(filePath);
            Debug.Log("<color=yellow><b>>>> End Capture</b></color>");
        }

        string CleanF (float num)
            => num < 0.001f ? "0" : $"{num:F3}";

        string CleanV3(Vector3 vector3)
        {
            var result = $"({CleanF(vector3.x)},{CleanF(vector3.y)},{CleanF(vector3.z)})";
            result = result.Replace(".000", "")
                .Replace("00)", ")")
                .Replace("(1,1,1)", "(one)")
                .Replace("(0,0,0)", "(zero)");
            return result;
        }

        List<string> CaptureChunkContent(Vector3 chunkPos, Vector3 chunkSize)
        {
            var bounds = new Bounds(chunkPos, chunkSize);
            var allRenderers = FindObjectsOfType<Renderer>();

            return allRenderers.Where(renderer => bounds.Intersects(renderer.bounds))
                .Where(renderer => !blackList.Contains(renderer.gameObject.name))
                .Select(renderer => new { renderer, rendTransform = renderer.transform })
                .Select(tuple =>
                    $"{tuple.renderer.gameObject.name}, " +
                    $"pos:{CleanV3(tuple.rendTransform.position)}, " +
                    $"eul: {CleanV3(tuple.rendTransform.eulerAngles)}, " +
                    $"scale: {CleanV3(tuple.rendTransform.localScale)}")
                .ToList();
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
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using JetBrains.Annotations;
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
        [FolderPath, SerializeField] string savePath;


        [Button(ButtonSizes.Medium)]
        async void Capture ()
        {
            Debug.Log($"<color=cyan><b>>>> Start Capture</b></color>");

            var data = CaptureSceneData();
            Debug.Log(data.Objects.Count);
            var fileName = $"{data.TimeStamp}_{data.CameraPosition}_{data.CameraRotation}";
            var filePath = JsonUtilities.SaveAsJsonFile(savePath, fileName, data);

            await ImageCapture.Capture(savePath, fileName);

            DoEditorRefreshAndSelect(filePath);

            Debug.Log($"<color=yellow><b>>>> End Capture</b></color>");
        }

        CameraCapturedSceneData CaptureSceneData()
        {
            var cam = SceneView.lastActiveSceneView.camera;
            var camTransform = cam.transform;

            var capturedData = new CameraCapturedSceneData
            {
                TimeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm"),
                CameraPosition = new Vector3Serializable(camTransform.position),
                CameraRotation = new QuaternionSerializable(camTransform.rotation)
            };

            var planes = GeometryUtility.CalculateFrustumPlanes(cam);

            var allRenderers = FindObjectsOfType<Renderer>();
            foreach (var renderer in allRenderers)
            {
                if (!GeometryUtility.TestPlanesAABB(planes, renderer.bounds)) continue;

                var rendTransform = renderer.transform;
                var sceneObjectData = new SceneObjectData
                {
                    ObjectName = renderer.gameObject.name,
                    ObjectPosition = new Vector3Serializable(rendTransform.position),
                    ObjectRotation = new QuaternionSerializable(rendTransform.rotation),
                    ObjectScale = new Vector3Serializable(rendTransform.localScale)
                };

                if (PrefabUtility.IsPartOfAnyPrefab(renderer))
                {
                    var prefabRootName = PrefabUtility.GetNearestPrefabInstanceRoot(renderer).name;
                    sceneObjectData.PrefabRoot = prefabRootName;
                }

                var components = renderer.gameObject.GetComponents<Component>();
                foreach (var component in components)
                {
                    sceneObjectData.Components.Add(component.GetType().ToString());
                }

                capturedData.Objects.Add(sceneObjectData);
            }

            return capturedData;
        }

        void DoEditorRefreshAndSelect (string path)
        {
            AssetDatabase.Refresh();
            var exportedAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
            EditorGUIUtility.PingObject(exportedAsset);
            Selection.activeObject = exportedAsset;
        }

        async Task<List<LabelAnnotation>> GoogleCloudVisionAnalyse(GoogleCloudVisionSettingsSo settings)
        {
            var cam = SceneView.lastActiveSceneView.camera.transform;
            var fileName = $"{cam.position}_{cam.rotation.eulerAngles}_{DateTime.Now:yyyy-MM-dd_HH-mm}";
            var imagePath = await ImageCapture.Capture(savePath, fileName);
            var api = new ImageRecognitionApi() as IImageRecognitionApi;
            return await api.AnalyzeImage(imagePath, settings, true);
        }
    }
}
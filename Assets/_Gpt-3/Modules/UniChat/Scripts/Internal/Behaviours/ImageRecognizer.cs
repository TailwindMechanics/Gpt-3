using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using System.IO;
using System;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.So;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.Internal.Apis;
using UnityEditor;


namespace Modules.UniChat.Internal.Behaviours
{
    [CreateAssetMenu(fileName = "new _imageRecognizer", menuName = "Tailwind/Google Cloud Vision/Image Recognizer")]
    public class ImageRecognizer : ScriptableObject
    {
        [FolderPath, SerializeField]
        string savePath;

        [PropertyOrder(2),SerializeField]
        List<LabelAnnotation> labels;

        [InlineEditor, SerializeField]
        GoogleCloudVisionSettingsSo settings;

        [UsedImplicitly]
        bool inProgress;


        [Button(ButtonSizes.Medium), HorizontalGroup("Buttons"), DisableIf("$inProgress")]
        async void Analyze()
        {
            inProgress = true;

            var imagePath = await Capture();
            var api = new ImageRecognitionApi() as IImageRecognitionApi;
            labels = await api.AnalyzeImage(imagePath, settings, true);

            inProgress = false;
        }

        [Button(ButtonSizes.Medium), HorizontalGroup("Buttons"), DisableIf("$inProgress")]
        void Clear () => labels.Clear();

        async Task<string> Capture()
        {
            Debug.Log("Capturing image...");

            var cam = SceneView.lastActiveSceneView.camera;
            var width = cam.pixelWidth;
            var height = cam.pixelHeight;

            var renderTexture = new RenderTexture(width, height, 24);
            cam.targetTexture = renderTexture;
            var capturedImage = new Texture2D(width, height, TextureFormat.RGB24, false);

            cam.Render();
            RenderTexture.active = renderTexture;
            capturedImage.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            capturedImage.Apply();

            var bytes = capturedImage.EncodeToPNG();
            var filePath = Path.Combine(savePath, CameraInfo(cam.transform));
            await File.WriteAllBytesAsync(filePath, bytes);

            cam.targetTexture = null;
            RenderTexture.active = null;
            AssetDatabase.Refresh();

            Debug.Log($"Image saved to {filePath}");

            return filePath;
        }

        string CameraInfo (Transform cam)
            => $"{cam.position}_{cam.rotation.eulerAngles}_{DateTime.Now:yyyy-MM-dd_HH-mm}.png";
    }
}
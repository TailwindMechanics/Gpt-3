using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using System.IO;


namespace Modules.UniChat.External.DataObjects
{
	public static class ImageCapture
	{
		public static async Task<string> Capture(string folderPath, string fileName)
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
			var filePath = Path.Combine(folderPath, $"{fileName}.png");
			await File.WriteAllBytesAsync(filePath, bytes);

			cam.targetTexture = null;
			RenderTexture.active = null;
			AssetDatabase.Refresh();

			Debug.Log($"Image saved to {filePath}");

			return filePath;
		}
	}
}
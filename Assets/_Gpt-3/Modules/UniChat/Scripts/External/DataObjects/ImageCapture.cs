using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using System.IO;


namespace Modules.UniChat.External.DataObjects
{
	public static class ImageCapture
	{
		public static async Task<string> Capture(Camera cam, string folderPath, string fileName, Vector2Int resolution)
		{
			var renderTexture = new RenderTexture(resolution.x, resolution.y, 24);
			cam.targetTexture = renderTexture;
			var capturedImage = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);

			cam.Render();
			RenderTexture.active = renderTexture;
			capturedImage.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
			capturedImage.Apply();

			var bytes = capturedImage.EncodeToPNG();
			var filePath = Path.Combine(folderPath, $"{fileName}.png");
			await File.WriteAllBytesAsync(filePath, bytes);

			cam.targetTexture = null;
			RenderTexture.active = null;
			AssetDatabase.Refresh();

			return filePath;
		}
	}
}
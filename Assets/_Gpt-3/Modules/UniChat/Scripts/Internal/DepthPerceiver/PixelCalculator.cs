using UnityEngine;


namespace Modules.UniChat.Internal.DepthPerceiver
{
	public class PixelCalculator
	{
		public float CalculatePixelPercentage(ObjectData objectData, Camera cam)
		{
			var screenBounds = CalculateScreenBounds(objectData, cam);
			var objectArea = screenBounds.size.x * screenBounds.size.y;
			var totalArea = cam.pixelWidth * cam.pixelHeight;
			var percentage = objectArea / totalArea * 100f;
			return percentage;
		}

		Bounds CalculateScreenBounds(ObjectData objectData, Camera cam)
		{
			var screenCenter = cam.WorldToScreenPoint(objectData.WorldPosition.Value());
			var objectPosition = objectData.WorldPosition.Value();
			var objectSize = objectData.Size.Value();
			var screenSize = new Vector3(
				Vector3.Distance(cam.WorldToScreenPoint(objectPosition - objectSize * 0.5f),
					cam.WorldToScreenPoint(objectPosition + objectSize * 0.5f)),
				Vector3.Distance(cam.WorldToScreenPoint(objectPosition - objectSize * 0.5f),
					cam.WorldToScreenPoint(objectPosition + objectSize * 0.5f)), 0);
			return new Bounds(screenCenter, screenSize);
		}
	}
}
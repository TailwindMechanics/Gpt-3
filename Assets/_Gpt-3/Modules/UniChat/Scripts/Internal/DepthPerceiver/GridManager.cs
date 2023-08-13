using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.DepthPerceiver
{
	public class GridManager
	{
		public SceneObjects GetSceneObjects(Camera cam, Transform player, AiPerceptionSettingsVo settings)
		{
			var frustumObjects = new FrustumCaptor().CaptureObjectsInFrustum(cam, player, settings.MaxSightDistance);
			var pixelCalculator = new PixelCalculator();
			var result = new SceneObjects();

			foreach (var obj in frustumObjects)
			{
				obj.PixelPercentage = pixelCalculator.CalculatePixelPercentage(obj, cam);
				var distance = obj.Direction.Value().magnitude;

				if (distance <= 1)
					result.WithinOne.Add(NlpReadable(obj));
				else if (distance <= 3)
					result.WithinThree.Add(NlpReadable(obj));
				else if (distance <= 5)
					result.WithinFive.Add(NlpReadable(obj));
				else if (distance <= 10)
					result.WithinTen.Add(NlpReadable(obj));
				else if (distance > 10 && obj.PixelPercentage > settings.MinPixelThreshold)
					result.Beyond.Add(NlpReadable(obj));
			}

			return result;
		}

		string NlpReadable(ObjectData objectData)
			=> $"{objectData.Name}:" +
			   $"direction:[{objectData.Direction.X:F2}m,{objectData.Direction.Y:F2}m,{objectData.Direction.Z:F2}m]," +
			   $"D:[{objectData.Size.Y:F2}m,{objectData.Size.X:F2}m,{objectData.Size.Z:F2}m]H:{objectData.AbsoluteHeading:F2}°";
	}
}
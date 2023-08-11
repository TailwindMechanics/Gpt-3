using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.DepthPerceiver
{
	public class GridManager
	{
		public SceneObjects GetSceneObjects(Camera cam, AiPerceptionSettingsVo settings)
		{
			var frustumObjects = new FrustumCaptor()
				.CaptureObjectsInFrustum(cam, settings.MaxSightDistance);

			var filteredObjects = new PixelCalculator()
				.FilterObjectsByPixelThreshold(frustumObjects, cam, settings.MinPixelThreshold);

			var result = new SceneObjects();
			foreach (var objectData in filteredObjects)
			{
				var distance = objectData.DirectionFromCamera.Value().magnitude;

				if (distance <= 1)
					result.WithinOne.Add(objectData.Name);
				else if (distance <= 3)
					result.WithinThree.Add(objectData.Name);
				else if (distance <= 5)
					result.WithinFive.Add(objectData.Name);
				else if (distance <= 10)
					result.WithinTen.Add(objectData.Name);
				else if (distance > 15)
					result.Beyond.Add(objectData.Name);
			}

			return result;
		}
	}
}
using System.Linq;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;
using Modules.Utilities.External;


namespace Modules.UniChat.Internal.DepthPerceiver
{
    public class GridManager
    {
        public SceneObjects GetSceneObjects(Camera cam, AiPerceptionSettingsVo settings)
        {
            var totalTokens = 0;
            Range currentRange = null;
            var result = new SceneObjects();
            var pixelCalculator = new PixelCalculator();
            var frustumObjects = new FrustumCaptor().CaptureObjectsInFrustum(cam);
            var sortedObjects = frustumObjects.OrderBy(o => o.Direction.Value().magnitude).ToList();

            foreach (var obj in sortedObjects)
            {
                obj.PixelPercentage = pixelCalculator.CalculatePixelPercentage(obj, cam);
                var distance = obj.Direction.Value().magnitude;
                var newItem = NlpReadable(obj);
                var newTokenCount = newItem.ApproxTokenCount();

                if (totalTokens + newTokenCount >= settings.MaxTokens) break;

                if (currentRange == null || !IsWithinLogRange(distance, double.Parse(currentRange.MinDistance)))
                {
                    currentRange = new Range { MinDistance = NextLogRangeStart(distance).ToString("F2") };
                    result.Ranges.Add(currentRange);
                }

                currentRange.Objects.Add(newItem);
                totalTokens += newTokenCount;
                currentRange.MaxDistance = distance.ToString("F2");
            }

            return result;
        }

        string NlpReadable(ObjectData objectData)
            => $"{objectData.Name}: " +
               $"position:[{objectData.WorldPosition.X:F2},{objectData.WorldPosition.Y:F2},{objectData.WorldPosition.Z:F2}]m, " +
               $"size:[{objectData.Size.Y:F2},{objectData.Size.X:F2},{objectData.Size.Z:F2}m]";
        bool IsWithinLogRange(double currentDistance, double minDistance)
            => currentDistance < 2 * minDistance;
        double NextLogRangeStart(float currentDistance)
            => Mathf.Pow(2, Mathf.Ceil(Mathf.Log(currentDistance, 2)));
    }
}
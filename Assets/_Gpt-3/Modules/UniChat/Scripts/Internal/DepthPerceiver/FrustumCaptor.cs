using System.Collections.Generic;
using Sirenix.Utilities;
using System.Linq;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.DepthPerceiver
{
	public class FrustumCaptor
    {
        public List<ObjectData> CaptureObjectsInFrustum(Camera cam, double maxDistance)
        {
            var allRenderers = Object.FindObjectsOfType<Renderer>();

            return allRenderers
                .Where(renderer => IsWithinCameraFrustum(cam, renderer, maxDistance))
                .Where(renderer => !LabelRules.Blacklist.Contains(renderer.gameObject.name))
                .Select(renderer => CreateObjectData(cam, renderer))
                .ToList();
        }

        bool IsWithinCameraFrustum(Camera cam, Renderer renderer, double maxDistance)
        {
            var bounds = renderer.bounds;
            var centerToCam = cam.transform.position - bounds.center;
            if (centerToCam.magnitude > maxDistance) return false;

            var planes = GeometryUtility.CalculateFrustumPlanes(cam);
            return GeometryUtility.TestPlanesAABB(planes, bounds);
        }

        ObjectData CreateObjectData(Camera cam, Renderer renderer)
        {
            var objectName = StripUnityLabels(renderer.name);
            var bounds = renderer.bounds;
            var direction = bounds.center - cam.transform.position;
            var size = bounds.size;

            return new ObjectData
            {
                Name = objectName,
                WorldPosition = new Vector3Serializable(bounds.center),
                DirectionFromCamera = new Vector3Serializable(direction),
                Size = new Vector3Serializable(size)
            };
        }

        string StripUnityLabels(string input)
        {
            input = input.Replace("(Clone)", "");
            for (var i = 0; i < 10; i++)
            {
                input = input.Replace($"({i})", "").Replace($"_0{i}", "");
            }

            LabelRules.RemoveFromNames.ForEach(item =>
            {
                if (input.Contains(item))
                {
                    input = input.Replace(item, "");
                }
            });

            return input.Trim().Replace(" ", "_");
        }
    }
}
using System.Collections.Generic;
using Sirenix.Utilities;
using System.Linq;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;
using Modules.Utilities.External;


namespace Modules.UniChat.Internal.DepthPerceiver
{
	public class FrustumCaptor
    {
        public List<ObjectData> CaptureObjectsInFrustum(Camera cam, Transform player, double maxDistance)
        {
            var allRenderers = Object.FindObjectsOfType<Renderer>();

            return allRenderers
                .Where(renderer => IsWithinCameraFrustum(cam, renderer, maxDistance))
                .Where(renderer => IsObjectVisible(cam, renderer))
                .Where(renderer => !LabelRules.Blacklist.Contains(renderer.gameObject.name))
                .Select(renderer => CreateObjectData(player, renderer))
                .ToList();
        }

        bool IsWithinCameraFrustum(Camera cam, Renderer renderer, double maxDistance)
        {
            var bounds = renderer.bounds;
            var camPos = cam.transform.position;
            var dir = camPos - bounds.center;

            if (dir.magnitude > maxDistance) return false;

            var planes = GeometryUtility.CalculateFrustumPlanes(cam);
            var worldCorners = GetWorldSpaceBoxCorners(renderer);
            return worldCorners.All(corner => IsPointInsideFrustum(planes, corner));
        }

        bool IsObjectVisible(Camera cam, Renderer renderer)
        {
            var dir = renderer.bounds.center - cam.transform.position;
            if (Physics.Raycast(cam.transform.position, dir, out var hit))
            {
                return hit.transform == renderer.transform;
            }

            return false;
        }

        bool IsPointInsideFrustum(Plane[] planes, Vector3 point)
            => planes.All(plane => !(plane.GetDistanceToPoint(point) < 0));

        Vector3[] GetWorldSpaceBoxCorners(Renderer renderer)
        {
            var meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null) return null;
            var localBounds = meshFilter.sharedMesh.bounds;

            Vector3[] localCorners = {
                new (localBounds.min.x, localBounds.min.y, localBounds.min.z),
                new (localBounds.min.x, localBounds.min.y, localBounds.max.z),
                new (localBounds.min.x, localBounds.max.y, localBounds.min.z),
                new (localBounds.min.x, localBounds.max.y, localBounds.max.z),
                new (localBounds.max.x, localBounds.min.y, localBounds.min.z),
                new (localBounds.max.x, localBounds.min.y, localBounds.max.z),
                new (localBounds.max.x, localBounds.max.y, localBounds.min.z),
                new (localBounds.max.x, localBounds.max.y, localBounds.max.z),
            };

            for (var i = 0; i < localCorners.Length; i++)
            {
                localCorners[i] = renderer.transform.TransformPoint(localCorners[i]);
            }

            return localCorners;
        }

        ObjectData CreateObjectData(Transform player, Renderer renderer)
        {
            var objectName = StripUnityLabels(renderer.name);
            var bounds = renderer.bounds;
            var worldDirection = bounds.center - player.position;
            var localDirection = player.InverseTransformDirection(worldDirection);
            var size = bounds.size;

            return new ObjectData
            {
                Name = objectName,
                WorldPosition = new Vector3Serializable(bounds.center),
                RelativePosition = CalculateRelativePosition(bounds.center, player),
                Direction = new Vector3Serializable(localDirection),
                AbsoluteHeading = renderer.transform.eulerAngles.y.NormalizeDegrees(),
                Size = new Vector3Serializable(size)
            };
        }

        Vector3Serializable CalculateRelativePosition (Vector3 worldPos, Transform player)
        {
            var relativePos = worldPos - player.position;
            var localPos = player.InverseTransformDirection(relativePos);
            return new Vector3Serializable(localPos);
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
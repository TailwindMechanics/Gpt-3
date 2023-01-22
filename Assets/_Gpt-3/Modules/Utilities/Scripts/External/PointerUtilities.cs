using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Modules.Utilities.External
{
	public static class RaycastUtilities
	{
		// ReSharper disable once PossibleNullReferenceException
		static Ray ScreenPointToRay(Vector2 screenPos)
			=> Camera.main.ScreenPointToRay(screenPos);

		public static Vector3 ScreenPointToRayWorldPoint (Vector2 screenPos, out bool wasHit, LayerMask mask)
		{
			var ray = ScreenPointToRay(screenPos);
			wasHit = Physics.Raycast(ray.origin, ray.direction, out var hit, 10000f, mask.value);
			return wasHit
				? hit.point
				: Vector3.zero;
		}

		public static bool PointerIsOverUI(Vector2 screenPos)
		{
			var hitObject = UIRaycast(ScreenPosToPointerData(screenPos));
			return hitObject != null && hitObject.layer == LayerMask.NameToLayer("UI");
		}

		static GameObject UIRaycast (PointerEventData pointerData)
		{
			var results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerData, results);

			return results.Count < 1 ? null : results[0].gameObject;
		}

		static PointerEventData ScreenPosToPointerData (Vector2 screenPos)
			=> new(EventSystem.current){position = screenPos};
	}
}
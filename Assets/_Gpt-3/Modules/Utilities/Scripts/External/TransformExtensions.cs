using System.Linq;
using UnityEngine;


namespace Tailwind.Utilities.External
{
	public static class TransformExtensions
	{
		public static void ZeroOutLocalPositionAndRotation(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}

		public static Transform FindRecursive (this Transform transform, string query)
		{
			var hierarchy = transform.GetComponentsInChildren<Transform>(true).ToList();
			hierarchy.Remove(transform);
			return hierarchy.FirstOrDefault(item => item.name == query);
		}
	}
}
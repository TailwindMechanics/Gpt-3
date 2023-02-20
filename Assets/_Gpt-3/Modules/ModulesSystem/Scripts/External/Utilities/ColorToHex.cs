using UnityEngine;

namespace Modules.ModulesSystem.External.Utilities
{
	public static class ColorToHex
	{
		public static string ToHex(this Color color)
			=>  $"#{(byte)(color.r * 255f):X2}{(byte)(color.g * 255f):X2}{(byte)(color.b * 255f):X2}";
	}
}
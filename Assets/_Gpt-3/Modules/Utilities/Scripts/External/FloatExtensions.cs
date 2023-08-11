using System.Linq;

namespace Modules.Utilities.External
{
	public static class FloatExtensions
	{
		public static float GetAverage (this float query, ref int index, ref float[] array)
		{
			index++;
			index %= array.Length;
			array[index] = query;
			return array.Average();
		}

		public static float NormalizeDegrees(this float inputDegrees)
		{
			if (inputDegrees is >= 0 and <= 360) return inputDegrees;
			return (inputDegrees % 360 + 360) % 360;
		}
	}
}
using System.Linq;


namespace Tailwind.Utilities.External
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
	}
}
namespace Modules.Utilities.External
{
	public static class StringExtensions
	{
		public static string PrefixBelowTen(this int input)
			=> input < 10 ? $"0{input}" : input.ToString();
		public static int ApproxTokenCount(this string input)
			=> input.Length / 3;
		public static string Ellipses(this string input, int maxLength = 20)
		{
			input = input.Replace("\n", "").Replace("\r", "");
			return input.Length > maxLength ? $"{input.Substring(0, maxLength)}..." : input;
		}
	}
}
namespace Modules.Utilities.External
{
	public static class StringUtilities
	{
		public static string PrefixBelowTen(int input)
			=> input < 10 ? $"0{input}" : input.ToString();

		public static string Ellipses(string input, int maxLength = 20)
		{
			input = input.Replace("\n", "").Replace("\r", "");
			return input.Length > maxLength ? $"{input.Substring(0, maxLength)}..." : input;
		}
	}
}
namespace Modules.Utilities.External
{
	public static class StringUtilities
	{
		public static string PrefixBelowTen(int input)
			=> input < 10 ? $"0{input}" : input.ToString();
	}
}
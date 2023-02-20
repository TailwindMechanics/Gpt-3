

namespace Modules.Utilities.External
{
	public static class IntExtensions
	{
		public static int Increment (this int input, int arrayCount)
			=> Wrap(++input, arrayCount);
		public static int Decrement (this int input, int arrayCount)
			=> Wrap(--input, arrayCount);
		static int Wrap (int input, int arrayCount)
			=> (input % arrayCount + arrayCount) % arrayCount;
	}
}
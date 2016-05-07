using Spackle;

namespace RandomGeneration
{
	public sealed class Randomness
	{
		public int GetValue(int start, int end)
		{
			return new SecureRandom().Next(start, end);
		}
	}
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RandomGeneration.Tests
{
	[TestClass]
	public sealed class RandomnessTests
	{
		[TestMethod]
		public void GetValue()
		{
			var value = new Randomness().GetValue(2, 10);
			Assert.IsTrue(value >= 2);
			Assert.IsTrue(value <= 10);
		}
	}
}

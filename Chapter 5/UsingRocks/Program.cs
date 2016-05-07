using Moq;
using Rocks;
using Rocks.Options;
using System.Diagnostics;

namespace UsingRocks
{
	class Program
	{
		static void Main(string[] args)
		{
			Program.MockUsingMoq();
			Program.MockUsingRocks();
			Program.MockUsingRocksInDebug();
		}

		private static void MockUsingMoq()
		{
			var service = new Mock<IService>(MockBehavior.Strict);
			service.Setup(_ => _.GetId()).Returns(2);

			var user = new ServiceUser(service.Object);
			Debug.Assert(user.Id == 2);

			service.VerifyAll();
		}

		private static void MockUsingRocks()
		{
			var service = Rock.Create<IService>();
			service.Handle(_ => _.GetId()).Returns(2);

			var user = new ServiceUser(service.Make());
			Debug.Assert(user.Id == 2);

			service.Verify();
		}

		private static void MockUsingRocksInDebug()
		{
			var service = Rock.Create<IService>(
				new RockOptions(
					level: OptimizationSetting.Debug,
					codeFile: CodeFileOptions.Create));
			service.Handle(_ => _.GetId()).Returns(2);

			var user = new ServiceUser(service.Make());
			Debug.Assert(user.Id == 2);

			service.Verify();
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
	public class Class1
	{
		public void DoIt(Stream stream) { }
		public class Class4 { }
	}

	public class Class2
	{
		public Guid ProduceThis(Stream stream)
		{
			return Guid.NewGuid();
		}
	}

	namespace SubNamespace
	{
		public class Class3
		{
		}
	}
}

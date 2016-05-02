using System;
using System.IO;

namespace IntegrationTests
{
	public class Class2
	{
		public Guid ProduceThis(Stream stream)
		{
			return Guid.NewGuid();
		}
	}
}
using System;

namespace SimpleExamples
{
	class Program
	{
		private const string FinalValueMessage = "Final value: ";

		static void Main(string[] args)
		{
			decimal value = 0;

			if(args.Length > 0 && decimal.TryParse(args[0], out value))
			{
				decimal finalValue = Calculate(value);
				Console.Out.WriteLine(FinalValueMessage);
				Console.Out.WriteLine(finalValue.ToString());
			}
		}

		private static decimal Calculate(decimal value)
		{
			return 2 * value + 0.5M;
		}
	}
}

namespace Company.Product.Core
{
	public class One { /* ... */ }
	public class Two { /* ... */ }

	namespace SubNamespace
	{
		public class Three { /* ... */ }
	}
}
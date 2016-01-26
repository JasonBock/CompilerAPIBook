using Microsoft.CodeAnalysis;
using System;

namespace CompilerAPIMemberCount
{
	class Program
	{
		static void Main(string[] args)
		{
			var assembly = typeof(CSharpExtensions).Assembly;

			Console.Out.WriteLine($"Assembly {assembly.FullName}");

			var types = assembly.GetExportedTypes();

			Console.Out.WriteLine($"Type count {types.Length}");

			var methodCount = 0;

			foreach(var type in types)
			{
				methodCount += type.GetMethods().Length;
			}

			Console.Out.WriteLine($"Method count {methodCount}");
		}
	}
}

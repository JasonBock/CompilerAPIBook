using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using System.Reflection;

namespace CompileHelloWorld
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var code =
@"using System;

namespace HelloWorld
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Out.WriteLine(""Hello compiled world"");
		}
	}
}";

			var tree = SyntaxFactory.ParseSyntaxTree(code);
			var compilation = CSharpCompilation.Create(
				"HelloWorldCompiled.exe",
				options: new CSharpCompilationOptions(OutputKind.ConsoleApplication),
				syntaxTrees: new[] { tree },
				references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

			using (var stream = new MemoryStream())
			{
				var compileResult = compilation.Emit(stream);
				var assembly = Assembly.Load(stream.GetBuffer());
				assembly.EntryPoint.Invoke(null, BindingFlags.NonPublic | BindingFlags.Static, 
					null, new object[] { null }, null);
			}
		}
	}
}

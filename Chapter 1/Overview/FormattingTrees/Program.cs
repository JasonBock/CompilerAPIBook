using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using System;

namespace FormattingTrees
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			Program.FormatClassNode();
		}

		private static void FormatClassNode()
		{
			Console.Out.WriteLine(nameof(Program.FormatClassNode));
			var code = SyntaxFactory.ClassDeclaration("NewClass");
			Console.Out.WriteLine(code);
			Console.Out.WriteLine(code.NormalizeWhitespace());
			Console.Out.WriteLine(Formatter.Format(code, new AdhocWorkspace()));
			Console.Out.WriteLine(Formatter.Format(code, MSBuildWorkspace.Create()));
			Console.Out.WriteLine(code.WithAdditionalAnnotations(Formatter.Annotation));
		}
	}
}

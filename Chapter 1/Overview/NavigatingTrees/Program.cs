using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace NavigatingTrees
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var code = @"
using System;

public class ContainsMethods
{
	public void Method1() { }
	public void Method2(int a, Guid b) { }
	public void Method3(string a) { }
	public void Method4(ref string a) { }
}";

			var tree = SyntaxFactory.ParseSyntaxTree(code);
			Console.Out.WriteLine(
				tree.GetRoot().DescendantNodesAndTokensAndSelf(_ => true, true).Count());

			//Program.PrintMethodContentViaTree(tree);
			//Console.Out.WriteLine();
			//Program.PrintMethodContentViaWalker(tree);
			//Console.Out.WriteLine();
			Program.PrintMethodContentViaSemanticModel(tree);
		}

		private static void PrintMethodContentViaSemanticModel(SyntaxTree tree)
		{
			Console.Out.WriteLine(nameof(Program.PrintMethodContentViaSemanticModel));
			var compilation = CSharpCompilation.Create(
				"MethodContent",
				syntaxTrees: new[] { tree },
				references: new[]
				{
					MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
				});

			var model = compilation.GetSemanticModel(tree, true);

			var methods = tree.GetRoot().DescendantNodes(_ => true)
				.OfType<MethodDeclarationSyntax>();

			foreach (var method in methods)
			{
				var methodInfo = model.GetDeclaredSymbol(method) as IMethodSymbol;
				var parameters = new List<string>();

				foreach (var parameter in methodInfo.Parameters)
				{
					var isRef = parameter.RefKind == RefKind.Ref ? "ref " : string.Empty;
					parameters.Add($"{isRef}{parameter.Type.Name} {parameter.Name}");
				}

				Console.Out.WriteLine(
					$"{methodInfo.Name}({string.Join(", ", parameters)})");
			}
		}

		private static void PrintMethodContentViaWalker(SyntaxTree tree)
		{
			Console.Out.WriteLine(nameof(Program.PrintMethodContentViaWalker));

			new MethodWalker().Visit(tree.GetRoot());
		}

		private static void PrintMethodContentViaTree(SyntaxTree tree)
		{
			Console.Out.WriteLine(nameof(Program.PrintMethodContentViaTree));
			var methods = tree.GetRoot().DescendantNodes(_ => true)
				.OfType<MethodDeclarationSyntax>();

			foreach (var method in methods)
			{
				var parameters = new List<string>();

				foreach (var parameter in method.ParameterList.Parameters)
				{
					parameters.Add(
						$"{parameter.Type.ToFullString().Trim()} {parameter.Identifier.Text}");
				}

				Console.Out.WriteLine(
					$"{method.Identifier.Text}({string.Join(", ", parameters)})");
			}
		}
	}
}

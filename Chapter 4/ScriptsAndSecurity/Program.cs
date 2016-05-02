using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ScriptsAndSecurity
{
	class Program
	{
		static void Main(string[] args)
		{
			AsyncContext.Run(
				() => Program.MainAsync(args));
		}

		private static async Task MainAsync(string[] args)
		{
			File.WriteAllLines("secrets.txt", 
				new[] { "Secret password: 12345" });

			Console.Out.WriteLine(
				"Enter in your script - type \"STOP\" to quit:");
			var context = new ScriptingContext();
			var options = ScriptOptions.Default
				.AddImports(
					typeof(ImmutableArrayExtensions).Namespace)
				.AddReferences(
					typeof(ImmutableArrayExtensions).Assembly);

			while (true)
			{
				var code = Console.In.ReadLine();

				if (code == "STOP")
				{
					break;
				}
				else
				{
					var script = CSharpScript.Create(code, 
						globalsType: typeof(ScriptingContext),
						options: options);
					var compilation = script.GetCompilation();
					//var diagnostics = compilation.GetDiagnostics();
					var diagnostics = compilation.GetDiagnostics().Union(
						Program.VerifyCompilation(compilation)).ToImmutableArray();

					if (diagnostics.Length > 0)
					{
						foreach (var diagnostic in diagnostics)
						{
							Console.Out.WriteLine(diagnostic);
						}
					}
					else
					{
						var result = await CSharpScript.RunAsync(code,
							globals: context,
							options: options);

						if(result.ReturnValue != null)
						{
							Console.Out.WriteLine($"\t{result.ReturnValue}");
						}
					}
				}
			}
		}

		private static ImmutableArray<Diagnostic> VerifyCompilation(Compilation compilation)
		{
			var diagnostics = new List<Diagnostic>();

			foreach (var tree in compilation.SyntaxTrees)
			{
				var model = compilation.GetSemanticModel(tree);
				foreach (var node in tree.GetRoot().DescendantNodes(
					_ => true))
				{
					var symbol = model.GetSymbolInfo(node).Symbol;

					if (symbol != null)
					{
						var symbolNamespace = Program.GetFullNamespace(symbol);

						if(symbol.Kind == SymbolKind.Method ||
							symbol.Kind == SymbolKind.Property ||
							symbol.Kind == SymbolKind.NamedType)
						{
							if(symbol.Kind == SymbolKind.Method)
							{
								if (symbolNamespace == typeof(Person).Namespace &&
									symbol.ContainingType.Name == nameof(Person) &&
									symbol.Name == nameof(Person.Save))
								{
									diagnostics.Add(Diagnostic.Create(
										new DiagnosticDescriptor("SCRIPT02", "Persistence Error",
											"Cannot save a person",
											"Usage", DiagnosticSeverity.Error, false),
										node.GetLocation()));
								}
							}

							if (symbolNamespace == "System.IO" ||
								symbolNamespace == "System.Reflection")
							{
								diagnostics.Add(Diagnostic.Create(
									new DiagnosticDescriptor("SCRIPT01", "Inaccessable Member",
										"Cannot allow a member from namespace {0} to be used",
										"Usage", DiagnosticSeverity.Error, false),
									node.GetLocation(), symbolNamespace));
							}
						}
					}
				}
			}

			return diagnostics.ToImmutableArray();
		}

		private static string GetFullNamespace(ISymbol symbol)
		{
			var namespaces = new List<string>();
			var @namespace = symbol.ContainingNamespace;

			while(@namespace != null)
			{
				if(!string.IsNullOrWhiteSpace(@namespace.Name))
				{
					namespaces.Add(@namespace.Name);
				}

				@namespace = @namespace.ContainingNamespace;
			}

			namespaces.Reverse();

			return string.Join(".", namespaces);
		}
	}
}

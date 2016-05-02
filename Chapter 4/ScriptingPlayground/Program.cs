using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Nito.AsyncEx;
using ScriptingContext;
using System;
using System.Threading.Tasks;

namespace ScriptingPlayground
{
	class Program
	{
		static void Main(string[] args)
		{
			AsyncContext.Run(() => Program.MainAsync(args));
		}

		private static async Task MainAsync(string[] args)
		{
			await Program.ExecuteScriptsWithGlobalContextAsync();
		}

		private static async Task EvaluateCodeAsync()
		{
			Console.Out.WriteLine("Enter in your script:");
			var code = Console.In.ReadLine();
			Console.Out.WriteLine(
				await CSharpScript.EvaluateAsync(code));
		}

		private static async Task EvaluateCodeWithContextAsync()
		{
			Console.Out.WriteLine("Enter in your script:");
			var code = Console.In.ReadLine();
			Console.Out.WriteLine(
				await CSharpScript.EvaluateAsync(code, 
					options: ScriptOptions.Default
						.AddReferences(typeof(Context).Assembly)
						.AddImports(typeof(Context).Namespace)));
		}

		private static async Task EvaluateCodeWithGlobalContextAsync()
		{
			Console.Out.WriteLine("Enter in your script:");
			var code = Console.In.ReadLine();
			Console.Out.WriteLine(
				await CSharpScript.EvaluateAsync(code, 
					globals: new CustomContext(
						new Context(4), Console.Out)));
		}

		private static async Task CompileScriptAsync()
		{
			Console.Out.WriteLine("Enter in your script:");
			var code = Console.In.ReadLine();
			var script = CSharpScript.Create(code);
			var compilation = script.GetCompilation();
			var diagnostics = compilation.GetDiagnostics();

			if(diagnostics.Length > 0)
			{
				foreach (var diagnostic in diagnostics)
				{
					Console.Out.WriteLine(diagnostic);
				}
			}
			else
			{
				foreach (var tree in compilation.SyntaxTrees)
				{
					var model = compilation.GetSemanticModel(tree);
					foreach (var node in tree.GetRoot().DescendantNodes(
						_ => true))
					{
						var symbol = model.GetSymbolInfo(node).Symbol;
						Console.Out.WriteLine(
							$"{node.GetType().Name} {node.GetText().ToString()}");

						if (symbol != null)
						{
							var symbolKind = Enum.GetName(
								typeof(SymbolKind), symbol.Kind);
							Console.Out.WriteLine(
								$"\t{symbolKind} {symbol.Name}");
						}
					}
				}

				Console.Out.WriteLine((await script.RunAsync()).ReturnValue);
			}
		}

		private static async Task ExecuteScriptsWithStateAsync()
		{
			Console.Out.WriteLine("Enter in your script - type \"STOP\" to quit:");

			ScriptState<object> state = null;

			while (true)
			{
				var code = Console.In.ReadLine();

				if (code == "STOP")
				{
					break;
				}
				else
				{
					state = state == null ?
						await CSharpScript.RunAsync(code) :
						await state.ContinueWithAsync(code);

					foreach(var variable in state.Variables)
					{
						Console.Out.WriteLine(
							$"\t{variable.Name} - {variable.Type.Name}");
               }

					if (state.ReturnValue != null)
					{
						Console.Out.WriteLine(
							$"\tReturn value: {state.ReturnValue}");
               }
				}
			}
		}

		private static async Task ExecuteScriptsWithGlobalContextAsync()
		{
			Console.Out.WriteLine("Enter in your script - type \"STOP\" to quit:");

			var session = new DictionaryContext();

			while (true)
			{
				var code = Console.In.ReadLine();

				if (code == "STOP")
				{
					break;
				}
				else
				{
					var result = await CSharpScript.RunAsync(code,
						globals: session);

					if(result.ReturnValue != null)
					{
						Console.Out.WriteLine(
							$"\t{result.ReturnValue}");
					}
				}
			}
		}
	}
}

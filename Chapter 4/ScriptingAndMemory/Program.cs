using Microsoft.CodeAnalysis.CSharp.Scripting;
using Nito.AsyncEx;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ScriptingAndMemory
{
	class Program
	{
		static void Main(string[] args)
		{
			//Program.EvaluateRandomExpressions();
			AsyncContext.Run(
				() => Program.MainAsync(args));
		}

		private static async Task MainAsync(string[] args)
		{
			await EvaluateRandomScriptsAsync();
		}

		private static async Task EvaluateRandomScriptsAsync()
		{
			var random = new Random();
			var iterations = 0;
			var stopWatch = Stopwatch.StartNew();

			while (true)
			{
				var script = $@"({random.Next(1000)} + {random.Next(1000)}) * 
					{random.Next(10000)}";
				await CSharpScript.EvaluateAsync(script);
				iterations++;

				if (iterations == 1000)
				{
					stopWatch.Stop();
					Console.Out.WriteLine(
						$"{Environment.WorkingSet} - time: {stopWatch.Elapsed}");
					stopWatch = Stopwatch.StartNew();
               iterations = 0;
				}
			}
		}

		private static void EvaluateRandomExpressions()
		{
			var random = new Random();
			var iterations = 0;
			var stopWatch = Stopwatch.StartNew();

			while (true)
			{
				var lambda = Expression.Lambda(
					Expression.Multiply(
						Expression.Add(
							Expression.Constant(random.Next(1000)),
							Expression.Constant(random.Next(1000))),
						Expression.Constant(random.Next(10000))));
				(lambda.Compile() as Func<int>)();
				iterations++;

				if (iterations == 1000)
				{
					stopWatch.Stop();
					Console.Out.WriteLine(
						$"{Environment.WorkingSet} - time: {stopWatch.Elapsed}");
					stopWatch = Stopwatch.StartNew();
					iterations = 0;
				}
			}
		}
	}
}

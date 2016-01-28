using System;
using System.IO;

namespace CommentRemover.ConsoleApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			//System.Diagnostics.Debugger.Launch();

			if (args.Length == 0 || args[0] == string.Empty)
			{
				Console.Out.WriteLine(
					"Usage: CommentRemover.ConsoleApplication {solution or project file}");
			}

			var file = args[0];

			if (!File.Exists(file))
			{
				Console.Out.WriteLine($"File {file} does not exist.");
			}
			else
			{
				if (Path.GetExtension(file) == ".sln")
				{
					WorkspaceCommentRemover.RemoveCommentsFromSolutionAsync(file).Wait();
				}
				else if (Path.GetExtension(file) == ".csproj")
				{
					WorkspaceCommentRemover.RemoveCommentsFromProjectAsync(file).Wait();
				}
				else
				{
					Console.Out.WriteLine("Only .sln and .csproj files are supported.");
				}
			}
		}
	}
}

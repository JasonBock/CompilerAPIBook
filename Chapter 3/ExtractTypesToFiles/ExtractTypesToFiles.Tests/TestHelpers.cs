using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExtractTypesToFiles.Tests
{
	internal static class TestHelpers
	{
		internal static async Task TestProvider(string file, string fileName,
			Func<Solution, ProjectId, Solution> modifySolution,
			Func<ImmutableArray<CodeAction>, Task> handleActions)
		{
			var code = File.ReadAllText(file);
			var document = TestHelpers.CreateDocument(code, fileName, modifySolution);
			var actions = new List<CodeAction>();
			var actionRegistration = new Action<CodeAction>(action => actions.Add(action));
			var context = new CodeRefactoringContext(document, new TextSpan(0, 1),
				actionRegistration, new CancellationToken(false));

			var provider = new ExtractTypesToFilesCodeRefactoringProvider();
			await provider.ComputeRefactoringsAsync(context);
			await handleActions(actions.ToImmutableArray());
		}

		internal static Document CreateDocument(string code, string fileName,
			Func<Solution, ProjectId, Solution> modifySolution)
		{
			var projectName = "Test";
			var projectId = ProjectId.CreateNewId(projectName);

			var solution = new AdhocWorkspace()
				 .CurrentSolution
				 .AddProject(projectId, projectName, projectName, LanguageNames.CSharp)
				 .AddMetadataReference(projectId,
					MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
				 .AddMetadataReference(projectId,
					MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location))
				 .AddMetadataReference(projectId,
					MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location))
				 .AddMetadataReference(projectId,
					MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location));

			var documentId = DocumentId.CreateNewId(projectId);
			solution = solution.AddDocument(documentId, fileName, SourceText.From(code));

			if(modifySolution != null)
			{
				solution = modifySolution(solution, projectId);
			}

			return solution.GetProject(projectId).Documents.First();
		}
	}
}

using ExtractTypesToFiles.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExtractTypesToFiles
{
	[ExportCodeRefactoringProvider(LanguageNames.CSharp,
		Name = nameof(ExtractTypesToFilesCodeRefactoringProvider))]
	[Shared]
	internal class ExtractTypesToFilesCodeRefactoringProvider
		: CodeRefactoringProvider
	{
		public sealed override async Task ComputeRefactoringsAsync(
			CodeRefactoringContext context)
		{
			var document = context.Document;
			var documentFileNameWithoutExtension =
				Path.GetFileNameWithoutExtension(document.FilePath);

			var root = await document.GetSyntaxRootAsync(context.CancellationToken)
				.ConfigureAwait(false);
			var model = await document.GetSemanticModelAsync(context.CancellationToken)
				.ConfigureAwait(false);

			var typesToRemove = root.GetTypesToRemove(
				model, documentFileNameWithoutExtension);

			if (typesToRemove.Length > 1)
			{
				context.RegisterRefactoring(CodeAction.Create(
					"Move types to files in folders",
					async token => await ExtractTypesToFilesCodeRefactoringProvider.CreateFiles(
						document, root, model, typesToRemove,
						_ => _.Replace(".", "\\"), token)));
				context.RegisterRefactoring(CodeAction.Create(
					"Move types to files in current folder",
					async token => await ExtractTypesToFilesCodeRefactoringProvider.CreateFiles(
						document, root, model, typesToRemove,
						_ => string.Empty, token)));
			}
		}

		private static async Task<Solution> CreateFiles(Document document, SyntaxNode root,
			SemanticModel model, ImmutableArray<TypeToRemove> typesToRemove,
			Func<string, string> typeFolderGenerator,
			CancellationToken token)
		{
			var project = document.Project;
			var workspace = project.Solution.Workspace;

			project = ExtractTypesToFilesCodeRefactoringProvider.MoveTypeNodes(
				model, typesToRemove, typeFolderGenerator, project, token);

			var newRoot = root.RemoveNodes(
				typesToRemove.Select(_ => _.Declaration),
				SyntaxRemoveOptions.AddElasticMarker);

			var newSolution = project.Solution;
			var projectId = project.Id;
			newSolution = newSolution.WithDocumentSyntaxRoot(document.Id, newRoot);

			var newDocument = newSolution.GetProject(project.Id).GetDocument(document.Id);
			newRoot = await newDocument.GetSyntaxRootAsync(token);
			var newModel = await newDocument.GetSemanticModelAsync(token);
			var newUsings = newRoot.GenerateUsingDirectives(newModel);

			newRoot = newRoot.RemoveNodes(
				newRoot.DescendantNodes(_ => true).OfType<UsingDirectiveSyntax>(),
				SyntaxRemoveOptions.AddElasticMarker);

			newRoot = (newRoot as CompilationUnitSyntax)?.WithUsings(newUsings);
			return newSolution.WithDocumentSyntaxRoot(document.Id, newRoot);
		}

		private static Project MoveTypeNodes(SemanticModel model, ImmutableArray<TypeToRemove> typesToRemove,
			Func<string, string> typeFolderGenerator, Project project, CancellationToken token)
		{
			var projectName = project.Name;

			foreach (var typeToRemove in typesToRemove)
			{
				token.ThrowIfCancellationRequested();
				var fileName = $"{typeToRemove.Symbol.Name}.cs";

				var containingNamespace = typeToRemove.Symbol.GetContainingNamespace();
				var typeFolder = typeFolderGenerator(containingNamespace).Replace(
					projectName, string.Empty);

				if (typeFolder.StartsWith("\\"))
				{
					typeFolder = typeFolder.Remove(0, 1);
				}

				project = project.AddDocument(fileName,
					typeToRemove.Declaration.GetCompilationUnitForType(model, containingNamespace),
					folders: !string.IsNullOrWhiteSpace(typeFolder) ?
						new[] { typeFolder } : null).Project;
			}

			return project;
		}
	}
}
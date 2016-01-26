using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExtractTypesToFiles.Tests
{
	[TestClass]
	public sealed class ExtractTypesToFilesCodeRefactoringProviderTests
	{
		[TestMethod]
		public async Task RefactorWhenFileCollisionOccurs()
		{
			ProjectId projectId = null;
			ImmutableArray<DocumentId> docIds;
			var folder = nameof(ExtractTypesToFilesCodeRefactoringProviderTests);
			var fileName = nameof(RefactorWhenFileCollisionOccurs);

			await TestHelpers.TestProvider(
				$@"Targets\{folder}\{fileName}.cs", "Class1.cs",
				(solution, pid) =>
				{
					projectId = pid;
					var documentId = DocumentId.CreateNewId(projectId);
					solution = solution.AddDocument(documentId, "Class2.cs", string.Empty);
					docIds = solution.GetProject(pid).Documents
						.Select(_ => _.Id).ToImmutableArray();
					return solution;
				},
				async actions =>
				{
					Assert.AreEqual(2, actions.Length);

					foreach (var action in actions)
					{
						var operations = await action.GetOperationsAsync(default(CancellationToken));
						Assert.AreEqual(1, operations.Length);

						var appliedOperation = (operations[0] as ApplyChangesOperation);
						var changedSolution = appliedOperation.ChangedSolution;
						var changedProject = changedSolution.GetProject(projectId);
						var changedDocuments = changedProject.Documents.Where(
							_ => !docIds.Any(id => id == _.Id)).ToImmutableArray();

						Assert.AreEqual(2, changedDocuments.Length);

						foreach (var document in changedDocuments)
						{
							var text = await document.GetTextAsync();
							var textValue = new StringBuilder();

							using (var writer = new StringWriter(textValue))
							{
								text.Write(writer);
							}

							var resultFile = $@"Targets\{folder}\{fileName}{document.Name}";
							Assert.AreEqual(File.ReadAllText(resultFile), textValue.ToString());
						}
					}
				});
		}

		[TestMethod]
		public async Task RefactorWhenOnlyOneTypeIsDefined()
		{
			var folder = nameof(ExtractTypesToFilesCodeRefactoringProviderTests);
			var fileName = nameof(RefactorWhenOnlyOneTypeIsDefined);

			await TestHelpers.TestProvider(
				$@"Targets\{folder}\{fileName}.cs", "Class1.cs",
				null,
				actions =>
				{
					Assert.AreEqual(0, actions.Length);
					return Task.CompletedTask;
				});
		}
	}
}

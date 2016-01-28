using CommentRemover.Extensions;
using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;

namespace CommentRemover.Extension
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
	[Guid(CommentRemoverPackage.PackageGuidString)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules",
		"SA1650:ElementDocumentationMustBeSpelledCorrectly",
		Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
	public sealed class CommentRemoverPackage
		: Package
	{
		public const string PackageGuidString = "7e923ca1-8495-48f9-a429-0373e32500d1";

		private DTE dte;
		private DocumentEventsClass documentEvents;
		private VisualStudioWorkspace workspace;

		protected override void Initialize()
		{
			var model = this.GetService(typeof(SComponentModel)) as IComponentModel;
			this.workspace = model.GetService<VisualStudioWorkspace>();
			this.dte = this.GetService(typeof(DTE)) as DTE;
			this.documentEvents = this.dte.Events.DocumentEvents as DocumentEventsClass;

			this.documentEvents.DocumentSaved += this.OnDocumentSaved;

			base.Initialize();
		}

		protected override void Dispose(bool disposing)
		{
			this.documentEvents.DocumentSaved -= this.OnDocumentSaved;
			base.Dispose(disposing);
		}

		private void OnDocumentSaved(EnvDTE.Document dteDocument)
		{
			var documentIds = this.workspace.CurrentSolution.GetDocumentIdsWithFilePath(
				dteDocument.FullName);

			if(documentIds != null && documentIds.Length == 1)
			{
				var documentId = documentIds[0];
				var document = this.workspace.CurrentSolution.GetDocument(documentId);

				if (Path.GetExtension(document.FilePath) == ".cs")
				{
					SyntaxNode root = null;

					if (document.TryGetSyntaxRoot(out root))
					{
						var newRoot = root.RemoveComments();

						if (newRoot != root)
						{
							var newSolution = document.Project.Solution
								.WithDocumentSyntaxRoot(document.Id, newRoot);
							this.workspace.TryApplyChanges(newSolution);
							dteDocument.Save();
						}
					}
				}
			}
		}
	}
}

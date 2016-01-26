using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;

namespace MustInvokeBaseMethod.Analyzers
{
	[ExportCodeFixProvider(LanguageNames.CSharp)]
	[Shared]
	public sealed class ThrowsExceptionCodeFix
	  : CodeFixProvider
	{
		public override ImmutableArray<string> FixableDiagnosticIds
		{
			get
			{
				return ImmutableArray.Create("THROW0001");
			}
		}

		public override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public override Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			throw new NotSupportedException("I can't fix this!");
		}
	}
}

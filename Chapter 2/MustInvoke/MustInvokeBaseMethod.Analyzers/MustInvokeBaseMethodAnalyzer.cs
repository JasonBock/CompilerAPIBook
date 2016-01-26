using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace MustInvokeBaseMethod.Analyzers
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class MustInvokeBaseMethodAnalyzer 
		: DiagnosticAnalyzer
	{
		private static DiagnosticDescriptor rule = new DiagnosticDescriptor(
		  "MUST0001", "Find Overridden Methods That do Not Call the Base Class's Method",
		  "Virtual methods with [MustInvoke] must be invoked in overrides",
		  "Usage", DiagnosticSeverity.Error, true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
		{
			get
			{
				return ImmutableArray.Create(
				  MustInvokeBaseMethodAnalyzer.rule);
			}
		}

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction<SyntaxKind>(
			  this.AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
		}

		private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
		{
			var method = context.Node as MethodDeclarationSyntax;
			var model = context.SemanticModel;
			var methodSymbol = model.GetDeclaredSymbol(method) as IMethodSymbol;

			context.CancellationToken.ThrowIfCancellationRequested();

			if(methodSymbol.IsOverride)
			{
				var overriddenMethod = methodSymbol.OverriddenMethod;

				var hasAttribute = false;
				foreach (var attribute in overriddenMethod.GetAttributes())
				{
					if(attribute.AttributeClass.Name == "MustInvokeAttribute")
					{
						hasAttribute = true;
						break;
					}
				}

				context.CancellationToken.ThrowIfCancellationRequested();

				if (hasAttribute)
				{
					var invocations = method.DescendantNodes(_ => true)
						.OfType<InvocationExpressionSyntax>();

					foreach (var invocation in invocations)
					{
						var invocationSymbol = model.GetSymbolInfo(invocation.Expression).Symbol as IMethodSymbol;

						if (invocationSymbol == overriddenMethod)
						{
							return;
						}
					}

					context.ReportDiagnostic(Diagnostic.Create(
					  MustInvokeBaseMethodAnalyzer.rule, 
					  method.Identifier.GetLocation()));
				}
			}
		}
	}
}

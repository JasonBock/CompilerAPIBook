using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Reflection;

namespace MustInvokeBaseMethod.Analyzers
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class ThrowExceptionAnalyzer
		: DiagnosticAnalyzer
	{
		private static DiagnosticDescriptor rule = new DiagnosticDescriptor(
		  "THROW0001", "Returning Ints From Methods",
		  "Returning ints is a really bad idea.",
		  "Usage", DiagnosticSeverity.Error, true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
		{
			get
			{
				return ImmutableArray.Create(
				  ThrowExceptionAnalyzer.rule);
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
			var returnType = methodSymbol.ReturnType;
			var intType = typeof(int).GetTypeInfo();

			if (returnType.Name == intType.Name &&
				returnType.ContainingAssembly.Name == intType.Assembly.GetName().Name)
			{
				//throw new NotSupportedException("Returning ints is a really bad idea.");
				context.ReportDiagnostic(Diagnostic.Create(
				  ThrowExceptionAnalyzer.rule,
				  method.Identifier.GetLocation()));
			}
		}
	}
}

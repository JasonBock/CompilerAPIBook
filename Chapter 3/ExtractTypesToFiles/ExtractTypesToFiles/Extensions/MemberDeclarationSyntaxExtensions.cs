using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace ExtractTypesToFiles.Extensions
{
	internal static class MemberDeclarationSyntaxExtensions
	{
		internal static CompilationUnitSyntax GetCompilationUnitForType(
			this MemberDeclarationSyntax @this,
			SemanticModel model, string containingNamespace)
		{
			var usingsForType = @this.GenerateUsingDirectives(model);

			return SyntaxFactory.CompilationUnit()
				.WithUsings(usingsForType)
				.WithMembers(
					SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
						SyntaxFactory.NamespaceDeclaration(
							SyntaxFactory.IdentifierName(containingNamespace))
							.WithMembers(
								SyntaxFactory.List<MemberDeclarationSyntax>(new[] { @this }))))
				.WithAdditionalAnnotations(Formatter.Annotation);
		}
	}
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;

namespace ModifyingTrees
{
	public sealed class MethodRewriter
		: CSharpSyntaxRewriter
	{
		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			var visibilityTokens = node.DescendantTokens(_ => true)
				.Where(_ => _.IsKind(SyntaxKind.PublicKeyword) ||
					_.IsKind(SyntaxKind.PrivateKeyword) ||
					_.IsKind(SyntaxKind.ProtectedKeyword) ||
					_.IsKind(SyntaxKind.InternalKeyword)).ToImmutableList();

			if (!visibilityTokens.Any(_ => _.IsKind(SyntaxKind.PublicKeyword)))
			{
				var tokenPosition = 0;

				var newMethod = node.ReplaceTokens(visibilityTokens,
					(_, __) =>
					{
						tokenPosition++;

						return tokenPosition == 1 ?
							SyntaxFactory.Token(
								_.LeadingTrivia,
								SyntaxKind.PublicKeyword,
								_.TrailingTrivia) :
							new SyntaxToken();
					});
				return newMethod;
			}
			else
			{
				return node;
			}
		}
	}
}

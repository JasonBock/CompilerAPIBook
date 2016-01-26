using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ExtractTypesToFiles.Extensions
{
	internal static class SyntaxNodeExtensions
	{
		internal static ImmutableArray<TypeToRemove> GetTypesToRemove(
			this SyntaxNode @this, SemanticModel model, 
			string documentFileNameWithoutExtension)
		{
			var typesToRemove = new List<TypeToRemove>();
			TypeDeclarationSyntax typeToPreserve = null;

			var typeNodes = @this.DescendantNodes(_ => true)
				.OfType<TypeDeclarationSyntax>();

			foreach(var typeNode in typeNodes)
			{
				var type = model.GetDeclaredSymbol(typeNode) as ITypeSymbol;

				if(type.ContainingType == null)
				{
					if(type.Name != documentFileNameWithoutExtension)
					{
						typesToRemove.Add(new TypeToRemove(
							typeNode, type));
					}
					else
					{
						typeToPreserve = typeNode;
					}
				}
			}

			return typesToRemove.ToImmutableArray();
		}

		internal static SyntaxList<UsingDirectiveSyntax> GenerateUsingDirectives(
			this SyntaxNode @this, SemanticModel model)
		{
			var namespacesForType = new SortedSet<string>();

			foreach (var childNode in @this.DescendantNodes(_ => true))
			{
				var symbol = model.GetSymbolInfo(childNode).Symbol;

				if (symbol != null && symbol.Kind != SymbolKind.Namespace &&
					symbol.ContainingNamespace != null)
				{
					if ((symbol as ITypeSymbol)?.SpecialType ==
						SpecialType.System_Void)
					{
						continue;
					}

					var containingNamespace = symbol.GetContainingNamespace();

					if (!string.IsNullOrWhiteSpace(containingNamespace))
					{
						namespacesForType.Add(containingNamespace);
					}
				}
			}

			return SyntaxFactory.List(
				namespacesForType.Select(_ => SyntaxFactory.UsingDirective(
					SyntaxFactory.IdentifierName(_))));
		}
	}
}

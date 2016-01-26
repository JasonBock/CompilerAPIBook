using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExtractTypesToFiles
{
	internal sealed class TypeToRemove
	{
		public TypeToRemove(TypeDeclarationSyntax declaration,
			ITypeSymbol symbol)
		{
			this.Declaration = declaration;
			this.Symbol = symbol;
		}

		public TypeDeclarationSyntax Declaration { get; }
		public ITypeSymbol Symbol { get; }
	}
}

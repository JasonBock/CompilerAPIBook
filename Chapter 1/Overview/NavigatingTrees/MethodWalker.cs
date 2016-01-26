using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NavigatingTrees
{
	public sealed class MethodWalker
		: CSharpSyntaxWalker
	{
		public MethodWalker(SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node)
			: base(depth)
		{ }

		public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			var parameters = new List<string>();

			foreach (var parameter in node.ParameterList.Parameters)
			{
				parameters.Add($"{parameter.Type.ToFullString().Trim()} {parameter.Identifier.Text}");
			}

			Console.Out.WriteLine(
				$"{node.Identifier.Text}({string.Join(", ", parameters)})");

			base.VisitMethodDeclaration(node);
		}
	}
}

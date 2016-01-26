using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Collections.Immutable;

namespace ModifyingTrees
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var code = @"
using System;

public class ContainsMethods
{
	public void Method1() { }
	protected void Method2(int a, Guid b) { }
	internal void Method3(string a) { }
	private void Method4(ref string a) { }
	protected internal void Method5(long a) { }
}";

			var tree = SyntaxFactory.ParseSyntaxTree(code);

			Program.ModifyTreeViaTree(tree);
			Console.Out.WriteLine();
			Program.ModifyTreeViaRewriter(tree);
			Console.Out.WriteLine();
			Program.ModifyTreeViaTreeWithAnnotations(tree);
		}

		private static void ModifyTreeViaRewriter(SyntaxTree tree)
		{
			Console.Out.WriteLine(nameof(Program.ModifyTreeViaTree));

			Console.Out.WriteLine(tree);
			var newTree = new MethodRewriter().Visit(tree.GetRoot());
			Console.Out.WriteLine(newTree);
		}

		private static void ModifyTreeViaTree(SyntaxTree tree)
		{
			Console.Out.WriteLine(nameof(Program.ModifyTreeViaTree));
			Console.Out.WriteLine(tree);
			var methods = tree.GetRoot().DescendantNodes(_ => true)
				.OfType<MethodDeclarationSyntax>();

			var newTree = tree.GetRoot().ReplaceNodes(methods, (method, methodWithReplacements) =>
			{
				var visibilityTokens = method.DescendantTokens(_ => true)
					.Where(_ => _.IsKind(SyntaxKind.PublicKeyword) ||
						_.IsKind(SyntaxKind.PrivateKeyword) ||
						_.IsKind(SyntaxKind.ProtectedKeyword) ||
						_.IsKind(SyntaxKind.InternalKeyword)).ToImmutableList();

				if (!visibilityTokens.Any(_ => _.IsKind(SyntaxKind.PublicKeyword)))
				{
					var tokenPosition = 0;

					var newMethod = method.ReplaceTokens(visibilityTokens,
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
					return method;
				}
			});

			Console.Out.WriteLine(newTree);
		}

		private static void ModifyTreeViaTreeWithAnnotations(SyntaxTree tree)
		{
			const string newMethodAnnotation = "MethodMadePublic";

			Console.Out.WriteLine(nameof(Program.ModifyTreeViaTreeWithAnnotations));
			Console.Out.WriteLine(tree);
			var methods = tree.GetRoot().DescendantNodes(_ => true)
				.OfType<MethodDeclarationSyntax>();

			var newTree = tree.GetRoot().ReplaceNodes(methods, (method, methodWithReplacements) =>
			{
				var visibilityTokens = method.DescendantTokens(_ => true)
					.Where(_ => _.IsKind(SyntaxKind.PublicKeyword) ||
						_.IsKind(SyntaxKind.PrivateKeyword) ||
						_.IsKind(SyntaxKind.ProtectedKeyword) ||
						_.IsKind(SyntaxKind.InternalKeyword)).ToImmutableList();

				if (!visibilityTokens.Any(_ => _.IsKind(SyntaxKind.PublicKeyword)))
				{
					var tokenPosition = 0;

					var newMethod = method.ReplaceTokens(visibilityTokens,
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
					return newMethod.WithAdditionalAnnotations(
						new SyntaxAnnotation(newMethodAnnotation));
				}
				else
				{
					return method;
				}
			});

			Console.Out.WriteLine(newTree);
			Console.Out.WriteLine(
				$"Modified method count: {newTree.GetAnnotatedNodes(newMethodAnnotation).Count()}");
		}
	}
}
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Reflection;

namespace BuildingTrees
{
	public static class Program
	{
		private const string NamespaceName = "BuildingTrees";
		private const string ClassName = "Doubler";
		private const string MethodName = "Double";

		public static void Main(string[] args)
		{
			var unit = SyntaxFactory.CompilationUnit()
				.WithMembers(
					SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
						SyntaxFactory.NamespaceDeclaration(
							SyntaxFactory.IdentifierName(
								Program.NamespaceName))
						.WithNamespaceKeyword(
							SyntaxFactory.Token(
								SyntaxKind.NamespaceKeyword))
						.WithOpenBraceToken(
							SyntaxFactory.Token(
								SyntaxKind.OpenBraceToken))
						.WithMembers(
							SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
								SyntaxFactory.ClassDeclaration(
									Program.ClassName)
								.WithModifiers(
									SyntaxFactory.TokenList(
										new[]
										{
											SyntaxFactory.Token(
												SyntaxKind.PublicKeyword),
											SyntaxFactory.Token(
												SyntaxKind.StaticKeyword)
										}))
								.WithKeyword(
									SyntaxFactory.Token(
										SyntaxKind.ClassKeyword))
								.WithOpenBraceToken(
									SyntaxFactory.Token(
										SyntaxKind.OpenBraceToken))
								.WithMembers(
									SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
										SyntaxFactory.MethodDeclaration(
											SyntaxFactory.PredefinedType(
												SyntaxFactory.Token(
													SyntaxKind.IntKeyword)),
											SyntaxFactory.Identifier(
												Program.MethodName))
										.WithModifiers(
											SyntaxFactory.TokenList(
												new[]
												{
													SyntaxFactory.Token(
														SyntaxKind.PublicKeyword),
													SyntaxFactory.Token(
														SyntaxKind.StaticKeyword)
												}))
										.WithParameterList(
											SyntaxFactory.ParameterList(
												SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
													SyntaxFactory.Parameter(
														SyntaxFactory.Identifier(
															@"a"))
													.WithType(
														SyntaxFactory.PredefinedType(
															SyntaxFactory.Token(
																SyntaxKind.IntKeyword)))))
												.WithOpenParenToken(
													SyntaxFactory.Token(
														SyntaxKind.OpenParenToken))
												.WithCloseParenToken(
													SyntaxFactory.Token(
														SyntaxKind.CloseParenToken)))
										.WithBody(
											SyntaxFactory.Block(
												SyntaxFactory.SingletonList<StatementSyntax>(
													SyntaxFactory.ReturnStatement(
														SyntaxFactory.BinaryExpression(
															SyntaxKind.MultiplyExpression,
															SyntaxFactory.LiteralExpression(
																SyntaxKind.NumericLiteralExpression,
																SyntaxFactory.Literal(
																	SyntaxFactory.TriviaList(),
																	@"2",
																	2,
																	SyntaxFactory.TriviaList())),
																SyntaxFactory.IdentifierName(
																@"a"))
														.WithOperatorToken(
															SyntaxFactory.Token(
																SyntaxKind.AsteriskToken)))
													.WithReturnKeyword(
														SyntaxFactory.Token(
															SyntaxKind.ReturnKeyword))
													.WithSemicolonToken(
														SyntaxFactory.Token(
															SyntaxKind.SemicolonToken))))
											.WithOpenBraceToken(
												SyntaxFactory.Token(
													SyntaxKind.OpenBraceToken))
											.WithCloseBraceToken(
												SyntaxFactory.Token(
													SyntaxKind.CloseBraceToken)))))
									.WithCloseBraceToken(
										SyntaxFactory.Token(
											SyntaxKind.CloseBraceToken))))
							.WithCloseBraceToken(
								SyntaxFactory.Token(
									SyntaxKind.CloseBraceToken))))
					.WithEndOfFileToken(
						SyntaxFactory.Token(
							SyntaxKind.EndOfFileToken));
			var tree = SyntaxFactory.SyntaxTree(unit);
			
			var compilation = CSharpCompilation.Create(
				"Doubler.dll",
				options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
				syntaxTrees: new[] { tree },
				references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

			using (var stream = new MemoryStream())
			{
				var compileResult = compilation.Emit(stream);
				var assembly = Assembly.Load(stream.GetBuffer());

				var type = assembly.GetType($"{Program.NamespaceName}.{Program.ClassName}");
				var method = type.GetMethod(Program.MethodName);

				var result = (int)method.Invoke(null, new object[] { 2 });

				Console.Out.WriteLine(result);
			}
		}
	}
}

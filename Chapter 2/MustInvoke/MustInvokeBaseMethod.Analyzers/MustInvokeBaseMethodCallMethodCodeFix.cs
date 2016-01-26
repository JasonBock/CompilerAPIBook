using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace MustInvokeBaseMethod.Analyzers
{
	[ExportCodeFixProvider(LanguageNames.CSharp)]
	[Shared]
	public sealed class MustInvokeBaseMethodCallMethodCodeFix
	  : CodeFixProvider
	{
		public override ImmutableArray<string> FixableDiagnosticIds
		{
			get
			{
				return ImmutableArray.Create("MUST0001");
			}
		}

		public override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(
				context.CancellationToken).ConfigureAwait(false);

			context.CancellationToken.ThrowIfCancellationRequested();

			var diagnostic = context.Diagnostics[0];
			var methodNode = root.FindNode(diagnostic.Location.SourceSpan) as MethodDeclarationSyntax;

			var model = await context.Document.GetSemanticModelAsync(context.CancellationToken);
			var methodSymbol = model.GetDeclaredSymbol(methodNode) as IMethodSymbol;

			// Syntax tree way.
			//var invocation = MustInvokeBaseMethodCallMethodCodeFix.CreateInvocation(
			//	methodSymbol);
			//invocation = MustInvokeBaseMethodCallMethodCodeFix.AddArguments(
			//	context, methodSymbol, invocation);
			//var statement = MustInvokeBaseMethodCallMethodCodeFix.CreateStatement(
			//	context, methodNode, methodSymbol, invocation);

			// Code parsing way
			var statement = MustInvokeBaseMethodCallMethodCodeFix.CreateStatement(
				methodNode, methodSymbol);

			var newRoot = MustInvokeBaseMethodCallMethodCodeFix.CreateNewRoot(
				root, methodNode, statement);

			const string codeFixDescription = "Add base invocation";
			context.RegisterCodeFix(
			  CodeAction.Create(codeFixDescription,
				 _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
				 codeFixDescription), diagnostic);
		}

		private static StatementSyntax CreateStatement(MethodDeclarationSyntax methodNode, IMethodSymbol methodSymbol)
		{
			var methodParameters = methodSymbol.Parameters;
			var arguments = new string[methodParameters.Length];

			for(var i = 0; i < methodParameters.Length; i++)
			{
				var parameter = methodParameters[i];
				var argument = parameter.Name;

				if (parameter.RefKind.HasFlag(RefKind.Ref))
				{
					argument = $"ref {argument}";
				}
				else if (parameter.RefKind.HasFlag(RefKind.Out))
				{
					argument = $"out {argument}";
				}

				arguments[i] = argument;
			}

			var methodCall = $"base.{methodSymbol.Name}({string.Join(", ", arguments)});{Environment.NewLine}";

			if(!methodSymbol.ReturnsVoid)
			{
				var variableName = MustInvokeBaseMethodCallMethodCodeFix.CreateSafeLocalVariableName(
					methodNode, methodSymbol);
				methodCall = $"var {variableName} = {methodCall}";
			}

			return SyntaxFactory.ParseStatement(methodCall).WithAdditionalAnnotations(Formatter.Annotation);
		}

		private static InvocationExpressionSyntax CreateInvocation(IMethodSymbol methodSymbol)
		{
			return SyntaxFactory.InvocationExpression(
				SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
					SyntaxFactory.BaseExpression().WithToken(
						SyntaxFactory.Token(SyntaxKind.BaseKeyword)),
						SyntaxFactory.IdentifierName(
							methodSymbol.Name))
				.WithOperatorToken(
						SyntaxFactory.Token(
							SyntaxKind.DotToken)));
		}

		private static SyntaxNode CreateNewRoot(SyntaxNode root, MethodDeclarationSyntax methodNode, StatementSyntax statement)
		{
			var body = methodNode.Body;
			var firstNode = body.ChildNodes().FirstOrDefault();

			var newBody = firstNode != null ?
				body.InsertNodesBefore(body.ChildNodes().First(),
					new[] { statement }) :
				SyntaxFactory.Block(statement);

			var newRoot = root.ReplaceNode(body, newBody);
			return newRoot;
		}

		private static StatementSyntax CreateStatement(CodeFixContext context, MethodDeclarationSyntax methodNode, 
			IMethodSymbol methodSymbol, InvocationExpressionSyntax invocation)
		{
			context.CancellationToken.ThrowIfCancellationRequested();

			StatementSyntax statement = null;

			// Now create a return value variable if needed; otherwise just call the method.
			if (!methodSymbol.ReturnsVoid)
			{
				var returnValueSafeName = CreateSafeLocalVariableName(methodNode, methodSymbol);

				statement = SyntaxFactory.LocalDeclarationStatement(
					SyntaxFactory.VariableDeclaration(
						SyntaxFactory.IdentifierName("var"))
					.WithVariables(SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
						SyntaxFactory.VariableDeclarator(
							SyntaxFactory.Identifier(returnValueSafeName))
					.WithInitializer(SyntaxFactory.EqualsValueClause(invocation)))));
			}
			else
			{
				statement = SyntaxFactory.ExpressionStatement(invocation);
			}

			return statement.WithAdditionalAnnotations(Formatter.Annotation);
		}

		private static string CreateSafeLocalVariableName(MethodDeclarationSyntax methodNode, IMethodSymbol methodSymbol)
		{
			var localDeclarations = methodNode.DescendantNodes(_ => true).OfType<VariableDeclaratorSyntax>();
			var returnValueName = $"{methodSymbol.Name.Substring(0, 1).ToLower()}{methodSymbol.Name.Substring(1)}Result";
			var returnValueSafeName = returnValueName;
			var returnValueCount = 0;

			// Create a "safe" local variable name.
			while (localDeclarations.Any(_ => _.Identifier.Text == returnValueSafeName))
			{
				returnValueSafeName = $"{returnValueName}{returnValueCount}";
				returnValueCount++;
			}

			return returnValueSafeName;
		}

		private static InvocationExpressionSyntax AddArguments(CodeFixContext context, 
			IMethodSymbol methodSymbol, InvocationExpressionSyntax invocation)
		{
			context.CancellationToken.ThrowIfCancellationRequested();

			var argumentCount = methodSymbol.Parameters.Length;
			if (argumentCount > 0)
			{
				// Define an argument list.
				var arguments = new SyntaxNodeOrToken[(2 * argumentCount) - 1];

				for (var i = 0; i < argumentCount; i++)
				{
					var parameter = methodSymbol.Parameters[i];
					var argument = SyntaxFactory.Argument(
						SyntaxFactory.IdentifierName(parameter.Name));

					if (parameter.RefKind.HasFlag(RefKind.Ref))
					{
						argument = argument.WithRefOrOutKeyword(
							SyntaxFactory.Token(SyntaxKind.RefKeyword));
					}
					else if (parameter.RefKind.HasFlag(RefKind.Out))
					{
						argument = argument.WithRefOrOutKeyword(
							SyntaxFactory.Token(SyntaxKind.OutKeyword));
					}

					arguments[2 * i] = argument;

					if (i < argumentCount - 1)
					{
						arguments[(2 * i) + 1] = SyntaxFactory.Token(SyntaxKind.CommaToken);
					}
				}

				invocation = invocation.WithArgumentList(
					SyntaxFactory.ArgumentList(
						SyntaxFactory.SeparatedList<ArgumentSyntax>(arguments))
					.WithOpenParenToken(SyntaxFactory.Token(SyntaxKind.OpenParenToken))
					.WithCloseParenToken(SyntaxFactory.Token(SyntaxKind.CloseParenToken)));
			}

			return invocation;
		}
	}
}

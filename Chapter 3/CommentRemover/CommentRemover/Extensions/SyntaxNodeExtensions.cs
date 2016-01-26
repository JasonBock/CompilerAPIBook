using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using System.Collections.Generic;
using System.Linq;

namespace CommentRemover.Extensions
{
	public static class SyntaxNodeExtensions
	{
		public static T RemoveComments<T>(this T @this)
			where T : SyntaxNode
		{
			var triviaToRemove = new List<SyntaxTrivia>();

			var nodesWithComments = @this.DescendantNodesAndTokens(_ => true)
				.Where(_ => _.HasLeadingTrivia &&
					_.GetLeadingTrivia().Any(__ =>
						__.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
						__.IsKind(SyntaxKind.MultiLineCommentTrivia)));

			foreach (var nodeWithComments in nodesWithComments)
			{
				var leadingTrivia = nodeWithComments.GetLeadingTrivia();
				var comments = leadingTrivia.Where(_ =>
					_.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
						_.IsKind(SyntaxKind.MultiLineCommentTrivia))
					.ToArray();
				triviaToRemove.AddRange(comments);
				triviaToRemove.AddRange(leadingTrivia.Where(_ =>
					_.IsKind(SyntaxKind.EndOfLineTrivia))
					.Take(comments.Length));
				triviaToRemove.AddRange(leadingTrivia.Where(_ =>
					_.IsKind(SyntaxKind.WhitespaceTrivia))
					.Take(comments.Length));
			}

			return triviaToRemove.Count > 0 ?
				@this.ReplaceTrivia(
					triviaToRemove, (_, __) => new SyntaxTrivia())
				.WithAdditionalAnnotations(Formatter.Annotation) :
				@this;
		}
	}
}

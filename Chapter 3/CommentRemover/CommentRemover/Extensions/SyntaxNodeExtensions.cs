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

			var commentCount = 0;

			foreach (var nodeWithComments in nodesWithComments)
			{
				var leadingTrivia = nodeWithComments.GetLeadingTrivia();

				for (var i = 0; i < leadingTrivia.Count; i++)
				{
					var trivia = leadingTrivia[i];

					if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
						trivia.IsKind(SyntaxKind.MultiLineCommentTrivia))
					{
						triviaToRemove.Add(trivia);
						commentCount++;

						if (i > 0)
						{
							var precedingTrivia = leadingTrivia[i - 1];

							if (precedingTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
							{
								triviaToRemove.Add(precedingTrivia);
							}
						}
					}
				}

				triviaToRemove.AddRange(leadingTrivia.Where(_ =>
					_.IsKind(SyntaxKind.EndOfLineTrivia))
					.Take(commentCount));
			}

			return triviaToRemove.Count > 0 ?
				@this.ReplaceTrivia(
					triviaToRemove, (_, __) => new SyntaxTrivia())
				.WithAdditionalAnnotations(Formatter.Annotation) :
				@this;
		}
	}
}

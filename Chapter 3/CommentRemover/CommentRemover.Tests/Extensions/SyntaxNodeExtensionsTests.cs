using CommentRemover.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommentRemover.Tests.Extensions
{
	[TestClass]
	public sealed class SyntaxNodeExtensionsTests
	{
		[TestMethod]
		public void RemoveCommentsFromCodeWithMultiLineComments()
		{
			var code =
@"public class Class
{
	public void Method()
	{
		/*
		Here is a multiline comment.
		*/
	}
}";

			var newCode =
@"public class Class
{
	public void Method()
	{
	}
}";
			var node = SyntaxFactory.ParseCompilationUnit(code);
			var newNode = node.RemoveComments();

			Assert.AreNotSame(node, newNode);
			Assert.AreEqual(newCode, newNode.GetText().ToString());
		}

		[TestMethod]
		public void RemoveCommentsFromCodeWithSingleLineComments()
		{
			var code =
@"public class Class
{
	public void Method()
	{
		// Here is a single line comment
		// Here is another single line comment
	}
}";

			var newCode =
@"public class Class
{
	public void Method()
	{
	}
}";
			var node = SyntaxFactory.ParseCompilationUnit(code);
			var newNode = node.RemoveComments();

			Assert.AreNotSame(node, newNode);
			Assert.AreEqual(newCode, newNode.GetText().ToString());
		}

		[TestMethod]
		public void RemoveCommentsFromCodeWithSingleLineCommentsWithoutWhitespace()
		{
			var code =
@"public class Class
{
	public void Method()
	{
// Here is a single line comment
	}
}";

			var newCode =
@"public class Class
{
	public void Method()
	{
	}
}";
			var node = SyntaxFactory.ParseCompilationUnit(code);
			var newNode = node.RemoveComments();

			Assert.AreNotSame(node, newNode);
			Assert.AreEqual(newCode, newNode.GetText().ToString());
		}

		[TestMethod]
		public void RemoveCommentsFromCodeWithMultiLineDocumentationComments()
		{
			var code =
@"public class Class
{
	/// <summary>
	/// Here are some XML comments
	/// </summary>
	public void Method()
	{
	}
}";

			var node = SyntaxFactory.ParseCompilationUnit(code);
			var newNode = node.RemoveComments();

			Assert.AreSame(node, newNode);
		}
	}
}

public class Class
{
	public void Method()
	{
		// Here is a single line comment
		// Here is another single line comment
	}
}
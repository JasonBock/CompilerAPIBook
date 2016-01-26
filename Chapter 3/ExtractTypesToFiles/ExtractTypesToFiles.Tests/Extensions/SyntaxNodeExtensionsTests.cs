using ExtractTypesToFiles.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExtractTypesToFiles.Tests.Extensions
{
	[TestClass]
	public sealed class SyntaxNodeExtensionsTests
	{
		[TestMethod]
		public async Task GenerateUsingDirectives()
		{
			var folder = nameof(SyntaxNodeExtensionsTests);
			var fileName = nameof(GenerateUsingDirectives);

			var document = TestHelpers.CreateDocument(
				File.ReadAllText($@"Targets\Extensions\{folder}\{fileName}.cs"),
				"Class1.cs", null);

			var root = await document.GetSyntaxRootAsync();
			var model = await document.GetSemanticModelAsync();

			var directives = root.GenerateUsingDirectives(model);
			Assert.AreEqual(3, directives.Count);
			directives.Single(_ => _.Name.ToString() == "System");
			directives.Single(_ => _.Name.ToString() == "System.IO");
			directives.Single(_ => _.Name.ToString() == "System.Text");
		}
	}
}

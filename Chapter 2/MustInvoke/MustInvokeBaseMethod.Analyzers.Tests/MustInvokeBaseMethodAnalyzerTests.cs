using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MustInvokeBaseMethod.Analyzers.Test
{
	[TestClass]
	public sealed class MustInvokeBaseMethodAnalyzerTests
	{
		[TestMethod]
		public void VerifySupportedDiagnostics()
		{
			var analyzer = new MustInvokeBaseMethodAnalyzer();
			var diagnostics = analyzer.SupportedDiagnostics;
			Assert.AreEqual(1, diagnostics.Length);

			var diagnostic = diagnostics[0];
			Assert.AreEqual(diagnostic.Title.ToString(),
				"Find Overridden Methods That do Not Call the Base Class's Method",
				nameof(DiagnosticDescriptor.Title));
			Assert.AreEqual(diagnostic.MessageFormat.ToString(),
				"Virtual methods with [MustInvoke] must be invoked in overrides",
				nameof(DiagnosticDescriptor.MessageFormat));
			Assert.AreEqual(diagnostic.Category,
				"Usage",
				nameof(DiagnosticDescriptor.Category));
			Assert.AreEqual(diagnostic.DefaultSeverity,
				DiagnosticSeverity.Error,
				nameof(DiagnosticDescriptor.DefaultSeverity));
		}

		[TestMethod]
		[Case("Case0")]
		public async Task AnalyzeWhenBaseClassHasNoVirtualMethods()
		{
			throw new NotImplementedException();
		}

		[TestMethod]
		[Case("Case1")]
		public async Task AnalyzeWhenBaseClassHasVirtualMethodWithoutMustInvokeButIsNotOverriden()
		{
			throw new NotImplementedException();
		}

		[TestMethod]
		[Case("Case2")]
		public async Task AnalyzeWhenBaseClassHasVirtualMethodWithoutMustInvokeAndIsOverridenWithBaseClassCall()
		{
			await TestHelpers.RunAnalysisAsync<MustInvokeBaseMethodAnalyzer>(
			  $@"Targets\{nameof(MustInvokeBaseMethodAnalyzerTests)}\Case2.cs",
			  new string[0]);
		}

		[TestMethod]
		[Case("Case3")]
		public async Task AnalyzeWhenBaseClassHasVirtualMethodWithoutMustInvokeAndIsNotOverridenWithBaseClassCall()
		{
			await TestHelpers.RunAnalysisAsync<MustInvokeBaseMethodAnalyzer>(
			  $@"Targets\{nameof(MustInvokeBaseMethodAnalyzerTests)}\Case3.cs",
			  new[] { "MUST0001" }, diagnostics =>
			  {
				  Assert.AreEqual(1, diagnostics.Count(), nameof(Enumerable.Count));
				  var diagnostic = diagnostics[0];
				  var span = diagnostic.Location.SourceSpan;
				  Assert.AreEqual(276, span.Start, nameof(span.Start));
				  Assert.AreEqual(282, span.End, nameof(span.End));
			  });
		}

		[TestMethod]
		[Case("Case4")]
		public async Task AnalyzeWhenBaseClassHasVirtualMethodWithMustInvokeButIsNotOverriden()
		{
			throw new NotImplementedException();
		}

		[TestMethod]
		[Case("Case4")]
		public async Task AnalyzeWhenBaseClassHasVirtualMethodWithMustInvokeAndIsOverridenWithBaseClassCall()
		{
			throw new NotImplementedException();
		}

		[TestMethod]
		[Case("Case5")]
		public async Task AnalyzeWhenBaseClassHasVirtualMethodWithMustInvokeAndIsNotOverridenWithBaseClassCall()
		{
			throw new NotImplementedException();
		}
	}
}
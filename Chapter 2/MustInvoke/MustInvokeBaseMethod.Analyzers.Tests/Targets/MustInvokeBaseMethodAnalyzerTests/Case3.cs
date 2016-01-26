using MustInvokeBaseMethod;

namespace MustInvokeBaseMethod.Analyzers.Test.Targets.MustInvokeBaseMethodAnalyzerTests
{
	public class Case3Base
	{
		[MustInvoke]
		public virtual void Method() { }
	}

	public class Case3Sub
		: Case3Base
	{
		public override void Method() { }
	}
}

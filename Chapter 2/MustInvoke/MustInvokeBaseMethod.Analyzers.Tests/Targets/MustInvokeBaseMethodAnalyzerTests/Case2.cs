using MustInvokeBaseMethod;

namespace MustInvokeBaseMethod.Analyzers.Test.Targets.MustInvokeBaseMethodAnalyzerTests
{
	public class Case2Base
	{
		[MustInvoke]
		public virtual void Method() { }
	}

	public class Case2Sub
		: Case3Base
	{
		public override void Method()
		{
			base.Method();
		}
	}
}

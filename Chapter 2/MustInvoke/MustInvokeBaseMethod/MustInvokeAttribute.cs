using System;

namespace MustInvokeBaseMethod
{
	[AttributeUsage(AttributeTargets.Method, 
		AllowMultiple = false, Inherited = true)]
	public sealed class MustInvokeAttribute
	  : Attribute
	{ }

	public class MyClass
	{
		protected virtual int[] OnInitialize(string a, Guid b, char c, ref long d, int e = 3) { return default(int[]); }
		protected virtual void OnThis() { }
	}

	public class MySub : MyClass
	{
		protected override void OnThis()
		{
			base.OnThis();
		}
		protected override int[] OnInitialize(string a, Guid b, char c, ref long d, int e = 3)
		{
			var onInitializeResult = base.OnInitialize(a, b, c, ref d, e);
			return onInitializeResult;
		}
	}
}

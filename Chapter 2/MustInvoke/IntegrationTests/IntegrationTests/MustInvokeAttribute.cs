using System;

namespace IntegrationTests
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class MustInvokeAttribute : Attribute { }
}

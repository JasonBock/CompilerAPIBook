using System;

namespace MustInvokeBaseMethod.Analyzers.Test
{
	[AttributeUsage(AttributeTargets.Method, 
		AllowMultiple = false,
		Inherited = false)]
	public sealed class CaseAttribute
		: Attribute
	{
		public CaseAttribute(string description)
		{
			this.Description = description;
		}

		public string Description { get; }
	}
}

using System;

namespace IntegrationTests
{
	public class MyBaseClass
	{
		[MustInvoke]
		protected virtual void OnInitialize() { }
	}

	public class MySubClass
	  : MyBaseClass
	{
		protected override void OnInitialize()
		{
		}
	}
}

using System.Collections.Generic;

namespace ScriptingContext
{
	public sealed class DictionaryContext
	{
		public DictionaryContext()
		{
			this.Values = new Dictionary<string, object>();
		}

		public Dictionary<string, object> Values { get; }
	}
}

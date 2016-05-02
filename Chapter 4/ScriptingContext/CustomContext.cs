using System.IO;

namespace ScriptingContext
{
	public class CustomContext
	{
		public CustomContext(Context context, TextWriter myOut)
		{
			this.Context = context;
			this.MyOut = myOut;
		}

		public Context Context { get; }
		public TextWriter MyOut { get; }
	}
}

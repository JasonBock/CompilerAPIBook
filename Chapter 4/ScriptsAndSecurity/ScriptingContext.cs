using System.Collections.Immutable;

namespace ScriptsAndSecurity
{
	public sealed class ScriptingContext
	{
		public ScriptingContext()
		{
			this.People = ImmutableArray.Create(
				new Person("Joe Smith", 30),
				new Person("Daniel Davis", 20),
				new Person("Sofia Wright", 25));
		}

		public ImmutableArray<Person> People { get; }
	}
}

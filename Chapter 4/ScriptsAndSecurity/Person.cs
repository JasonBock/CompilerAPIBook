namespace ScriptsAndSecurity
{
	public sealed class Person
	{
		public Person(string name, uint age)
		{
			this.Name = name;
			this.Age = age;
		}

		public void Save() { }

		public uint Age { get; set; }
		public string Name { get; set; }
	}
}

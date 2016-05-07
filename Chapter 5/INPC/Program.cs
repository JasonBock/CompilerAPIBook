using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace INPC
{
	class Program
	{
		public abstract class Properties
			: INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			protected Properties() { }

			protected virtual void OnPropertyChanged(string propertyName)
			{
				this.PropertyChanged?.Invoke(this,
					new PropertyChangedEventArgs(propertyName));
			}

			protected void SetField<T>(ref T field, T value, string propertyName)
			{
				if (!EqualityComparer<T>.Default.Equals(field, value))
				{
					field = value;
					this.OnPropertyChanged(propertyName);
				}
			}
		}

		public class IntegerData
			: Properties
		{
			private int value;
			public int Value
			{
				get { return this.value; }
				set { this.SetField(ref this.value, value, nameof(Value)); }
			}
		}

		private static void Main()
		{
			var properties = new IntegerData();
			properties.PropertyChanged +=
				(s, e) => Console.Out.WriteLine($"Property {e.PropertyName} changed.");
			Console.Out.WriteLine($"properties.Value is {properties.Value}");
			properties.Value = 2;
			Console.Out.WriteLine($"properties.Value is {properties.Value}");
			properties.Value = 3;
			Console.Out.WriteLine($"properties.Value is {properties.Value}");
			properties.Value = 3;
			Console.Out.WriteLine($"properties.Value is {properties.Value}");
			properties.Value = 4;
			Console.Out.WriteLine($"properties.Value is {properties.Value}");
		}
	}
}

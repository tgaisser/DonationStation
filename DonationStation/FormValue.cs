using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Signpost
{
	public class FormValue : INotifyPropertyChanged
	{
		private string stringValue;
		private int integerValue;

		public string StringValue
		{
			get { return stringValue; }
			set
			{
				stringValue = value;
				OnPropertyChanged();
			}
		}

		public int IntegerValue
		{
			get { return integerValue; }
			set
			{
				integerValue = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
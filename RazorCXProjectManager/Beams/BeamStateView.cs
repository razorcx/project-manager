using System;

namespace RazorCXProjectManager.Beams
{
	public class BeamStateView : PartCheckBase, ICloneable
	{
		private DateTime _date;
		public DateTime Date
		{
			get => _date;
			set
			{
				if (_date == value) return;
				_date = value;
				NotifyPropertyChanged("Date");
			}
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
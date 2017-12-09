using System.ComponentModel;

namespace RazorCXProjectManager.Beams
{
	public class ModelObjectCheck : INotifyPropertyChanged
	{
		public int Id { get; set; }

		private bool _isChanged;
		public bool IsChanged
		{
			get => _isChanged;
			set
			{
				if (_isChanged == value) return;
				_isChanged = value;
				NotifyPropertyChanged("IsChanged");
			}
		}

		private string _detailedBy;
		public string DetailedBy
		{
			get => _detailedBy;
			set
			{
				if (_detailedBy == value) return;
				_detailedBy = value;
				NotifyPropertyChanged("DetailedBy");
			}
		}

		private string _detailedDate;
		public string DetailedDate
		{
			get => _detailedDate;
			set
			{
				if (_detailedDate == value) return;
				_detailedDate = value;
				NotifyPropertyChanged("DetailedDate");
			}
		}
		private string _detailedComments;
		public string DetailedComments
		{
			get => _detailedComments;
			set
			{
				if (_detailedComments == value) return;
				_detailedComments = value;
				NotifyPropertyChanged("DetailedComments");
			}
		}

		private string _checkedBy;
		public string CheckedBy
		{
			get => _checkedBy;
			set
			{
				if (_checkedBy == value) return;
				_checkedBy = value;
				NotifyPropertyChanged("CheckedBy");
			}
		}

		private string _checkedDate;
		public string CheckedDate
		{
			get => _checkedDate;
			set
			{
				if (_checkedDate == value) return;
				_checkedDate = value;
				NotifyPropertyChanged("CheckedDate");
			}
		}

		private string _checkedComments;
		public string CheckedComments
		{
			get => _checkedComments;
			set
			{
				if (_checkedComments == value) return;
				_checkedComments = value;
				NotifyPropertyChanged("CheckedComments");
			}
		}

		private string _designedBy;
		public string DesignedBy
		{
			get => _designedBy;
			set
			{
				if (_designedBy == value) return;
				_designedBy = value;
				NotifyPropertyChanged("DesignedBy");
			}
		}

		private string _designedDate;
		public string DesignedDate
		{
			get => _designedDate;
			set
			{
				if (_designedDate == value) return;
				_designedDate = value;
				NotifyPropertyChanged("DesignedDate");
			}
		}

		private string _designedComments;
		public string DesignedComments
		{
			get => _designedComments;
			set
			{
				if (_designedComments == value) return;
				_designedComments = value;
				NotifyPropertyChanged("DesignedComments");
			}
		}

		private string _approvedBy;
		public string ApprovedBy
		{
			get => _approvedBy;
			set
			{
				if (_approvedBy == value) return;
				_approvedBy = value;
				NotifyPropertyChanged("ApprovedBy");
			}
		}

		private string _approvedDate;
		public string ApprovedDate
		{
			get => _approvedDate;
			set
			{
				if (_approvedDate == value) return;
				_approvedDate = value;
				NotifyPropertyChanged("ApprovedDate");
			}
		}

		private string _approvedComments;
		public string ApprovedComments
		{
			get => _approvedComments;
			set
			{
				if (_approvedComments == value) return;
				_approvedComments = value;
				NotifyPropertyChanged("ApprovedComments");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}
	}
}
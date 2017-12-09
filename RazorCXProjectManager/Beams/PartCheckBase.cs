namespace RazorCXProjectManager.Beams
{
	public class PartCheckBase : ModelObjectCheck
	{
		public int? Phase { get; set; }

		private string _name;
		public string Name
		{
			get => _name;
			set
			{
				if (_name == value) return;
				_name = value;
				NotifyPropertyChanged("Name");
			}
		}

		private string _profile;
		public string Profile
		{
			get => _profile;
			set
			{
				if (_profile == value) return;
				_profile = value;
				NotifyPropertyChanged("Profile");
			}
		}

		private string _material;
		public string Material
		{
			get => _material;
			set
			{
				if (_material == value) return;
				_material = value;
				NotifyPropertyChanged("Material");
			}
		}

		private string _finish;
		public string Finish
		{
			get => _finish;
			set
			{
				if (_finish == value) return;
				_finish = value;
				NotifyPropertyChanged("Finish");
			}
		}

		private string _class;
		public string Class
		{
			get => _class;
			set
			{
				if (_class == value) return;
				_class = value;
				NotifyPropertyChanged("Class");
			}
		}

		public string TopLevel { get; set; }
		public string PartMark { get; set; }
	}
}
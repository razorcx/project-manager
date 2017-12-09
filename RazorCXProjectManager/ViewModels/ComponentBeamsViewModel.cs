using System.Windows.Input;
using RazorCXProjectManager.Beams;
using RazorCXProjectManager.Common;

namespace RazorCXProjectManager.ViewModels
{
	public class ComponentBeamsViewModel : ViewModelBase
	{
		private readonly PartTypeEnum PartType = PartTypeEnum.CONNECTION_BEAMS;

		public ComponentBeamsViewModel()
		{
			InitializeBeamStateViews(PartType);
			UpdateManagedBeams(PartType);
		}

		public ICommand AddBeamsCommand => new DelegateCommand(AddBeams);
		private void AddBeams()
		{
			base.AddBeams(PartType);
		}

		public ICommand RemoveBeamsCommand => new DelegateCommand(RemoveBeams);
		private void RemoveBeams()
		{
			base.RemoveBeams(PartType);
		}

		public ICommand RefreshBeamsCommand => new DelegateCommand(RefreshBeams);
		private void RefreshBeams()
		{
			base.RefreshBeams(PartType);
		}
	}
}
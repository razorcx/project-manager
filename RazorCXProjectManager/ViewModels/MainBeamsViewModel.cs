using System.Windows.Controls;
using System.Windows.Input;
using RazorCXProjectManager.Beams;
using RazorCXProjectManager.Common;

namespace RazorCXProjectManager.ViewModels
{
	public class MainBeamsViewModel : ViewModelBase
	{
		private readonly PartTypeEnum PartType = PartTypeEnum.MAIN_BEAMS;

		public MainBeamsViewModel()
		{
			InitializeBeamStateViews(PartType);
			UpdateManagedBeams(PartType);
		}

		public void CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			CellEditEnding(sender, e, PartType);
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
using RazorCXProjectManager.Common;
using Tekla.Structures.Model;

namespace RazorCXProjectManager.ViewModels
{
	public class MainWindowViewModel : ObservableObject
	{
		private readonly Model _model = new Model();

		private ModelInfo _modelInfo;
		public ModelInfo ModelInfo
		{
			get => _modelInfo;
			set => _modelInfo = value;
		}

		private ProjectInfo _projectInfo;
		public ProjectInfo ProjectInfo
		{
			get => _projectInfo;
			set => _projectInfo = value;
		}

		public MainWindowViewModel()
		{
			_modelInfo = _model.GetInfo();
			_projectInfo = _model.GetProjectInfo();
		}
	}
}
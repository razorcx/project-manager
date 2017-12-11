using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Newtonsoft.Json;
using RazorCXProjectManager.Beams;
using RazorCXProjectManager.Common;
using Tekla.Structures.Model;
using Task = System.Threading.Tasks.Task;

namespace RazorCXProjectManager.ViewModels
{
	public abstract class ViewModelBase : ObservableObject
	{
		private ILookup<Guid, Beam> _beamsLookup;

		private string _managedBeams;
		public string ManagedBeams
		{
			get => _managedBeams;
			set
			{
				_managedBeams = value;
				RaisePropertyChangedEvent("ManagedBeams");
			}
		}

		private string _unManagedBeams;
		public string UnManagedBeams
		{
			get => _unManagedBeams;
			set
			{
				_unManagedBeams = value;
				RaisePropertyChangedEvent("UnManagedBeams");
			}
		}

		private ObservableCollection<BeamStateView> _beamStateViews;
		public ObservableCollection<BeamStateView> BeamStateViews
		{
			get => _beamStateViews;
			set
			{
				_beamStateViews = value;
				RaisePropertyChangedEvent("BeamStateViews");
			}
		}

		private BeamStateView _selectedBeamStateView;
	    public BeamStateView SelectedBeamStateView
	    {
		    get => _selectedBeamStateView;
		    set => _selectedBeamStateView = value;
	    }

	    private BeamStateView _selected;
	    public BeamStateView Selected
	    {
		    get => _selected;
		    set => _selected = value;
	    }

	    private ObservableCollection<object> _selectedItems;
	    public ObservableCollection<object> SelectedItems
	    {
		    get => _selectedItems;
		    set => _selectedItems = value;
	    }

		public readonly BeamManager BeamManager = new BeamManager();

		protected ViewModelBase()
		{
			_beamsLookup = GetBeamsLookup();
			SelectedItems = new ObservableCollection<object>();
		}

		public void InitializeBeamStateViews(PartTypeEnum partType)
		{
			BeamStateViews = new ObservableCollection<BeamStateView>(BeamManager.BeamStateViews(_beamsLookup, partType));
		}

		public ILookup<Guid, Beam> GetBeamsLookup()
		{
			return ModelHelper.GetAllBeams().ToLookup(b => b.Identifier.GUID);
		}

		public void AddBeams(PartTypeEnum partType)
		{
			var beams = BeamManager.AddBeams(partType);
			beams?.ForEach(b => BeamStateViews.Add(b));
			UpdateManagedBeams(partType);
		}

		public void RemoveBeams(PartTypeEnum partType)
		{
			if (!SelectedItems.Any()) return;

			var beamStateViews = SelectedItems.OfType<BeamStateView>().ToList();
			BeamManager.RemoveBeams(beamStateViews.Select(b => b.Id).ToList(), partType);

			var newStates = new List<BeamStateView>(BeamStateViews);
			beamStateViews.ForEach(b => newStates.Remove(b));
			if (!beamStateViews.Any()) BeamStateViews = new ObservableCollection<BeamStateView>();
			BeamStateViews = new ObservableCollection<BeamStateView>(newStates);
			UpdateManagedBeams(partType);
		}

		public void RefreshBeams(PartTypeEnum partType)
		{
			_beamsLookup = GetBeamsLookup();
			var newViews = BeamManager.BeamStateViews(_beamsLookup, partType).ToList();
			BeamStateViews = new ObservableCollection<BeamStateView>(newViews);
			UpdateManagedBeams(partType);
		}

		public void SelectMembersInModel()
		{
			var ids = SelectedItems
				.OfType<BeamStateView>()
				.Select(b => b.Id)
				.ToList();

			ModelHelper.SelectModelObjectsInUi(ids);
		}

		public void UpdateManagedBeams(PartTypeEnum partType)
		{
			Task.Run(() =>
			{
				ManagedBeams = BeamStateViews.Count.ToString();
				//UnManagedBeams = (BeamManager.GetBeams(partType).Count - BeamStateViews.Count).ToString();
			});
		}

		public void CellEditEnding(object sender, DataGridCellEditEndingEventArgs e, PartTypeEnum partType)
		{
			if (e.EditingElement.GetType().Name == "TextBox")
			{
				var value = ((TextBox)e.EditingElement).Text;
				var column = e.Column.Header.ToString().ToUpper();

				var beamState = BeamManager.GetBeamState(SelectedBeamStateView, column, value, partType);

				BeamManager.UpdateJsonFile(beamState, partType);
			}

			if (e.EditingElement.GetType().Name == "CheckBox")
			{
				bool? isChecked = true;

				isChecked = ((CheckBox)e.EditingElement).IsChecked;
				if (isChecked == true) return;

				SelectedBeamStateView.IsChanged = false;

				var beam = BeamManager.GetBeam(SelectedBeamStateView.Id);

				var beamSummaries = BeamManager.GetBeamSummaries(partType);

				var beamState = BeamManager.BeamStates(beamSummaries)
					.FirstOrDefault(b => b.Id == SelectedBeamStateView.Id);
				if (beamState == null) return;

				var newState = JsonConvert.DeserializeObject<BeamState>(JsonConvert.SerializeObject(SelectedBeamStateView));
				newState.Beam = beam;
				newState.Guid = beamState.Guid;

				var success = BeamManager.UpdateJsonFile(newState, partType);
			}
		}

		public void OpenDrawing(PartTypeEnum partType)
		{
			var part = ModelHelper.SelectModelObject(((BeamStateView)SelectedItems[0]).Id) as Part;
			new OpenShopDrawing().Open(part);
		}
	}
}
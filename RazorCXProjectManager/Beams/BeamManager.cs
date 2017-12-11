using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RazorCXProjectManager.Common;
using Tekla.Structures;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace RazorCXProjectManager.Beams
{
	public class BeamManager
	{
		private readonly Model _model;
		private readonly Picker _picker;
		private readonly Dictionary<string, string> _folderPaths = new Dictionary<string, string>();

		public BeamManager()
		{
			_model = new Model();
			_picker = new Picker();
			InitializePaths();
		}

		private void InitializePaths()
		{
			_folderPaths.Add("DATA_BEAMS", @"RazorCX\ProjectManager\Data\Beams\");
			_folderPaths.Add(PartTypeEnum.MAIN_BEAMS.ToString(), "MainBeams");
			_folderPaths.Add(PartTypeEnum.SECONDARY_BEAMS.ToString(), "SecondaryBeams");
			_folderPaths.Add(PartTypeEnum.CONNECTION_BEAMS.ToString(), "ComponentBeams");
		}

		public string GetPath(PartTypeEnum partType)
		{
			return Path.Combine(_folderPaths["DATA_BEAMS"], _folderPaths[partType.ToString()]);
		}

		public List<BeamStateView> AddBeams(PartTypeEnum partType)
		{
			try
			{
				var modelObjects = _picker.PickObjects(Picker.PickObjectsEnum.PICK_N_OBJECTS).ToList();
				if (modelObjects == null) return null;

				var path = GetPath(partType);
				var folderPath = FolderPath(path);
				var filenames = GetFilenames(folderPath);

				var objs = modelObjects
					.Where(m => filenames.FirstOrDefault(filename => filename.Contains(m.Identifier.GUID.ToString())) == null)
					.ToList();

				var beamStates = objs
					.OfType<Beam>()
					.FilterForPartType(partType)
					.Select(GetBeamState)
					.Where(b => b != null)
					.ToList();

				beamStates.ForEach(b =>
				{
					var beamSummary = new BeamSummary
					{
						GUID = b.Guid.ToString(),
						BeamStates = new List<BeamState> {b}
					};

					var json = JsonConvert.SerializeObject(beamSummary, Formatting.Indented);

					WriteStates(json, Filepath(folderPath, b.Guid));
				});

				var beamStateViews = JsonConvert.DeserializeObject<List<BeamStateView>>(JsonConvert.SerializeObject(beamStates))
					.OrderBy(b => b.Name)
					.ThenBy(b => b.Profile)
					.ToList();

				return beamStateViews;

			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public void RemoveBeams(List<int> ids, PartTypeEnum partType)
		{
			var modelObjects = ModelHelper.GetAllBeams();

			var beams = ids.Select(i => modelObjects.FirstOrDefault(b => b.Identifier.ID == i)).ToList();

			beams.ForEach(b => RemoveBeam(b, partType));
		}

		public void RemoveBeam(Beam beam, PartTypeEnum partType)
		{
			try
			{
				var path = GetPath(partType);
				var folderPath = FolderPath(path);
				var filename = Filepath(folderPath, beam.Identifier.GUID);
				File.Delete(filename);

			}
			catch (Exception e)
			{
			}
		}

		public List<BeamStateView> BeamStateViews(ILookup<Guid, Beam> beamsLookup, PartTypeEnum partType)
		{
			var beamSummaries = GetBeamSummaries(partType);
			var beamStates = BeamStates(beamSummaries);

			//compare
			var revised = new List<int>();
			beamStates.ForEach(b =>
			{
				var beam = beamsLookup[b.Guid].FirstOrDefault();
				if (beam != null)
				{

					var oldBeamCompare = JsonConvert.DeserializeObject<BeamCompare>(JsonConvert.SerializeObject(b));
					var newBeamCompare = new BeamCompare(beam);

					var oldBeam = JsonConvert.SerializeObject(oldBeamCompare);
					var newBeam = JsonConvert.SerializeObject(newBeamCompare);

					if (!oldBeam.Equals(newBeam)) revised.Add(beam.Identifier.ID);
				}
			});

			var beamsToAdd = new List<BeamState>();
			var beamsToRemove = new List<BeamState>();
			beamStates.ForEach(b =>
			{
				if (revised.Contains(b.Id))
				{
					var beam = (Beam) _model.SelectModelObject(new Identifier(b.Id));

					if (beam != null)
					{
						var beamState = GetBeamState(beam);
						beamState.IsChanged = true;
						beamsToRemove.Add(b);

						beamState.DetailedBy = b.DetailedBy;
						beamState.DetailedDate = b.DetailedDate;
						beamState.DetailedComments = b.DetailedComments;

						beamState.CheckedBy = b.CheckedBy;
						beamState.CheckedDate = b.CheckedDate;
						beamState.CheckedComments = b.CheckedComments;

						beamState.DesignedBy = b.DesignedBy;
						beamState.DesignedDate = b.DesignedDate;
						beamState.DesignedComments = b.DesignedComments;

						beamState.ApprovedBy = b.ApprovedBy;
						beamState.ApprovedDate = b.ApprovedDate;
						beamState.ApprovedComments = b.ApprovedComments;

						beamsToAdd.Add(beamState);
					}
				}
				else
				{
					b.IsChanged = false;
				}
			});

			beamsToRemove.ForEach(b => beamStates.Remove(b));
			beamStates.AddRange(beamsToAdd);
			beamsToAdd.ForEach(b => UpdateJsonFile(b, partType));

			var beamStateViews = JsonConvert.DeserializeObject<List<BeamStateView>>(JsonConvert.SerializeObject(beamStates))
				.OrderBy(b => b.Name)
				.ThenByDescending(b => b.Date)
				.ToList();

			return beamStateViews;
		}

		public List<Beam> GetBeams(PartTypeEnum partType)
		{
			return ModelHelper.GetAllBeams().FilterForPartType(partType);
		}

		public ManagedBeamsSummary ManagedBeamsSummary(PartTypeEnum partType)
		{
			var beams = ModelHelper.GetAllBeams()
				.AsParallel()
				.Where(b => b.GetAssembly().GetMainPart().Identifier.ID == b.Identifier.ID)
				.ToList();

			var beamSummaries = GetBeamSummaries(partType);

			return new ManagedBeamsSummary
			{
				Managed = beamSummaries.Count(b => b.BeamStates != null),
				UnManaged = beams.Count - beamSummaries.Count(b => b.BeamStates != null)
			};
		}

		public BeamState GetBeamState(BeamStateView beamStateView, string column, string value, PartTypeEnum partType)
		{
			var beamSummaries = GetBeamSummaries(partType);

			var beamState = BeamStates(beamSummaries)
				.FirstOrDefault(b => b.Id == beamStateView.Id);
			if (beamState == null) return null;

			beamState = UpdateCheckingProperties(beamState, column, value);

			var beam = GetBeam(beamStateView.Id);
			beam = UpdateMemberProperties(column, beam, value);
			if (beam != null)
				beamState.Beam = beam;

			return beamState;
		}

		public Beam GetBeam(int id)
		{
			return _model.SelectModelObject(new Identifier(id)) as Beam;
		}

		public Beam UpdateMemberProperties(string column, Beam beam, string value)
		{
			var commitChanges = false;

			switch (column)
			{
				case "PROFILE":
					beam.Profile = new Profile()
					{
						ProfileString = value
					};
					commitChanges = true;
					break;
				case "MATERIAL":
					beam.Material = new Material()
					{
						MaterialString = value
					};
					break;
				case "NAME":
					beam.Name = value;
					break;
				case "FINISH":
					beam.Finish = value;
					break;
				case "CLASS":
					beam.Class = value;
					commitChanges = true;
					break;
			}

			var success = beam.Modify();

			if(success && commitChanges)
				_model.CommitChanges();

			return beam;
		}

		public BeamState UpdateCheckingProperties(BeamState beamState, string column, string value)
		{
			if (beamState == null) return beamState;

			switch (column)
			{
				case "DETAILEDBY":
					beamState.DetailedBy = value;
					break;
				case "DETAILEDDATE":
					beamState.DetailedDate = value;
					break;
				case "DETAILEDCOMMENTS":
					beamState.DetailedComments = value;
					break;
				case "CHECKEDBY":
					beamState.CheckedBy = value;
					break;
				case "CHECKEDDATE":
					beamState.CheckedDate = value;
					break;
				case "CHECKEDCOMMENTS":
					beamState.CheckedComments = value;
					break;
				case "DESIGNEDBY":
					beamState.DesignedBy = value;
					break;
				case "DESIGNEDDATE":
					beamState.DesignedDate = value;
					break;
				case "DESIGNEDCOMMENTS":
					beamState.DesignedComments = value;
					break;
				case "APPROVEDBY":
					beamState.ApprovedBy = value;
					break;
				case "APPROVEDDATE":
					beamState.ApprovedDate = value;
					break;
				case "APPROVEDCOMMENTS":
					beamState.ApprovedComments = value;
					break;
			}

			return beamState;
		}

		private List<string> GetFilenames(string path)
		{
			return Directory.EnumerateFiles(path, "*.json").ToList();
		}

		private string FolderPath(string folder)
		{
			var modelpath = _model.GetInfo().ModelPath;
			var folderPath = modelpath + @"\" + folder;

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			return modelpath + @"\" + folder;
		}

		private string Filepath(string path, Guid guid)
		{
			var filename = $"{guid}.json";
			return path + @"\" + filename;
		}

		public BeamState GetBeamState(Beam beam)
		{
			beam.GetPhase(out Phase phase);

			var topLevel = string.Empty;
			beam.GetReportProperty("TOP_LEVEL", ref topLevel);

			return new BeamState()
			{
				Id = beam.Identifier.ID,
				Guid = beam.Identifier.GUID,
				Beam = beam,
				Phase = phase.PhaseNumber,
				Name = beam.Name,
				Profile = beam.Profile.ProfileString,
				Material = beam.Material.MaterialString,
				Finish = beam.Finish,
				Class = beam.Class,
				Date = DateTime.Now,
				TopLevel = topLevel,
				PartMark = beam.GetPartMark(),
			};
		}

		public List<BeamState> BeamStates(List<BeamSummary> beamSummaries)
		{
			return beamSummaries
				.Select(c =>
				{
					var result = c.BeamStates.LastOrDefault();
					return result ?? new BeamState();
				})
				.OrderByDescending(c => c.Date)
				.ToList();
		}

		public List<BeamSummary> GetBeamSummaries(PartTypeEnum partType)
		{
			var path = _folderPaths["DATA_BEAMS"] + _folderPaths[partType.ToString()];
			var folderPath = FolderPath(path);

			var fileNames = GetFilenames(folderPath);

			return fileNames.AsParallel().Select(GetBeamSummary).ToList();
		}

		public BeamSummary GetBeamSummary(string filepath)
		{
			var beamSummary = new BeamSummary();

			try
			{
				var file = string.Empty;

				using (var reader = new StreamReader(filepath))
					file = reader.ReadToEnd();

				if (!string.IsNullOrEmpty(file))
					beamSummary = JsonConvert.DeserializeObject<BeamSummary>(file);

				return beamSummary;
			}
			catch (Exception ex)
			{
				return beamSummary;
			}
		}

		private bool WriteStates(string json, string filepath)
		{
			try
			{
				using (var writer = new StreamWriter(filepath))
					writer.Write(json);
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public bool UpdateJsonFile(BeamState beamState, PartTypeEnum partType)
		{
			try
			{
				var path = GetPath(partType);
				var folderPath = FolderPath(path);
				var filePath = Filepath(folderPath, beamState.Guid);
				var file = string.Empty;

				using (var reader = new StreamReader(filePath))
					file = reader.ReadToEnd();
				if (string.IsNullOrEmpty(file)) return false;

				var beamSummary = JsonConvert.DeserializeObject<BeamSummary>(file);

				if (beamSummary.BeamStates == null) beamSummary.BeamStates = new List<BeamState>();

				beamSummary.BeamStates.Add(beamState);

				var json = JsonConvert.SerializeObject(beamSummary, Formatting.Indented);

				WriteStates(json, filePath);

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}
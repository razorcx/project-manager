using Tekla.Structures.Model;

namespace RazorCXProjectManager.Beams
{
	public class BeamCompare
	{
		public string Name { get; set; }
		public string Profile { get; set; }
		public string Material { get; set; }
		public string Finish { get; set; }
		public string Class { get; set; }
		public string TopLevel { get; set; }
		public string PartMark { get; set; }
		public int Phase { get; set; }

		public BeamCompare(Beam beam)
		{
			var topLevel = string.Empty;
			var phaseNumber = 0;

			if (beam.Identifier.ID > 0)
			{
				beam.GetPhase(out Phase phase);
				phaseNumber = phase.PhaseNumber;
				beam.GetReportProperty("TOP_LEVEL", ref topLevel);
			}

			Phase = phaseNumber;
			Name = beam.Name;
			Profile = beam.Profile.ProfileString;
			Material = beam.Material.MaterialString;
			Finish = beam.Finish;
			Class = beam.Class;
			TopLevel = topLevel;
			PartMark = beam.GetPartMark();
		}
	}
}
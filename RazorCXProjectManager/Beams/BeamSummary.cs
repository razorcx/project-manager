using System.Collections.Generic;

namespace RazorCXProjectManager.Beams
{
	public class BeamSummary
	{
		public string GUID { get; set; }
		public List<BeamState> BeamStates { get; set; }
	}
}
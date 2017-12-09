using System;
using Tekla.Structures.Model;

namespace RazorCXProjectManager.Beams
{
	public class BeamState : PartCheckBase
	{
		public Guid Guid { get; set; }
		public Beam Beam { get; set; }
		public DateTime? Date { get; set; }
	}
}
using RazorCXProjectManager.Beams;

namespace RazorCXProjectManager.Connections
{
	public class ConnectionCheckBase : ModelObjectCheck
	{
		public string Name { get; set; }
		public int? Number { get; set; }
		public int? Phase { get; set; }
	}
}
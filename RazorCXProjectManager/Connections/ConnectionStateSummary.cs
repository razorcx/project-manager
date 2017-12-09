using System.Collections.Generic;

namespace RazorCXProjectManager.Connections
{
	public class ConnectionStateSummary
	{
		public string GUID { get; set; }
		public List<ConnectionStateResult> ConnectionCheckResults { get; set; }
	}
}
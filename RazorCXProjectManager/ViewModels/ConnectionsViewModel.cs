using System.Collections.ObjectModel;
using RazorCXProjectManager.Connections;

namespace RazorCXProjectManager.ViewModels
{
	public class ConnectionsViewModel : ViewModelBase
	{
		private ObservableCollection<ConnectionStateView> _connectionStateViews;

		public ObservableCollection<ConnectionStateView> ConnectionStateViews => _connectionStateViews;

		private readonly ConnectionManager _connectionManager = new ConnectionManager();

		public ConnectionsViewModel()
		{
			_connectionStateViews = _connectionManager.ConnectionStateViews;
		}
	}
}
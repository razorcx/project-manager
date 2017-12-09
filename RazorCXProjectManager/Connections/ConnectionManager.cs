using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Tekla.Structures.Model;

namespace RazorCXProjectManager.Connections
{
	public class ConnectionManager
	{
		private readonly Model _model;

		public ObservableCollection<ConnectionStateView> ConnectionStateViews;

		public ConnectionManager()
		{
			_model = new Model();
			ConnectionStateViews = new ObservableCollection<ConnectionStateView>();
		}

		public List<ConnectionStateResult> ConnectionStateResults()
		{
			try
			{
				var connectionStateSummaries = GetConnectionStateSummaries();

				return connectionStateSummaries.Select(c =>
					{
						var result = c.ConnectionCheckResults.LastOrDefault();
						return result ?? new ConnectionStateResult();
					})
					.OrderByDescending(c => c.Date)
					.ToList();
			}
			catch
			{
				return new List<ConnectionStateResult>();
			}
		}

		public List<ConnectionStateSummary> GetConnectionStateSummaries()
		{
			var fileNames = GetConnectionFilenames();

			return fileNames.AsParallel().Select(f =>
				{
					var guid = f.Split('\\').LastOrDefault()?.Split('.').FirstOrDefault();
					var connectionCheckResults = ReadConnectionCheckHistory(f);

					return new ConnectionStateSummary
					{
						GUID = guid,
						ConnectionCheckResults = connectionCheckResults
					};
				})
				.ToList();
		}

		private List<string> GetConnectionFilenames()
		{
			var folder = ConnectionFolderPath();
			return Directory.EnumerateFiles(folder).ToList();
		}

		private string ConnectionFolderPath()
		{
			string modelpath = _model.GetInfo().ModelPath;
			var connectionCheckFolder = @"RazorCX\ConnectionChecker";
			return modelpath + @"\" + connectionCheckFolder;
		}

		private List<ConnectionStateResult> ReadConnectionCheckHistory(string filepath)
		{
			var checkConnections = new List<ConnectionStateResult>();

			try
			{
				var file = string.Empty;

				using (var reader = new StreamReader(filepath))
					file = reader.ReadToEnd();

				if (!string.IsNullOrEmpty(file))
					checkConnections =
						JsonConvert.DeserializeObject<List<ConnectionStateResult>>(file);

				return checkConnections;
			}
			catch (Exception ex)
			{
				return checkConnections;
			}
		}

		public ObservableCollection<ConnectionStateView> GetConnectionStateView(List<ConnectionStateResult> result)
		{
			var stateViews =
				result
					.Where(c => c != null)
					.Select(c => new ConnectionStateView
					{
						Id = c.Id,
						Phase = c.Phase,
						Name = c.Name,
						Number = c.Number,
						DetailedBy = c.DetailedBy,
						DetailedDate = c.DetailedDate,
						DetailedComments = c.DetailedComments,
						DesignedBy = c.DesignedBy,
						DesignedDate = c.DesignedDate,
						DesignedComments = c.DesignedComments,
						CheckedBy = c.CheckedBy,
						CheckedDate = c.CheckedDate,
						CheckedComments = c.CheckedComments,
						ApprovedBy = c.ApprovedBy,
						ApprovedDate = c.ApprovedDate,
						ApprovedComments = c.ApprovedComments,
						Date = c?.Date
					}).ToList();

			return new ObservableCollection<ConnectionStateView>(stateViews);
		}
	}
}
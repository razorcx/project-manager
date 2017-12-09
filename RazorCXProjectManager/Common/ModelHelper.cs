using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures;
using Tekla.Structures.Model;

namespace RazorCXProjectManager.Common
{
	public class ModelHelper
	{
		private static readonly Model Model = new Model();

		public static bool GetConnectionStatus()
		{
			return Model.GetConnectionStatus();
		}
		public static void SelectModelObjectsInUi(List<int> ids)
		{
			var modelObjects = new ArrayList();

			ids.ForEach(id =>
			{
				var modelObject = Model.SelectModelObject(new Identifier(id));
				if (modelObject == null) return;
				modelObjects.Add(modelObject);
			});

			var selector = new Tekla.Structures.Model.UI.ModelObjectSelector();
			selector.Select(modelObjects);
		}

		public static List<Connection> GetAllConnections()
		{
			ModelObjectEnumerator.AutoFetch = true;
			return Model.GetModelObjectSelector()
				.GetAllObjectsWithType(ModelObject.ModelObjectEnum.CONNECTION)
				.ToList()
				.OfType<Connection>()
				.ToList();
		}

		public static List<Beam> GetAllBeams()
		{
			ModelObjectEnumerator.AutoFetch = true;
			return Model.GetModelObjectSelector()
				.GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM)
				.ToList()
				.OfType<Beam>()
				.ToList();
		}
	}
}
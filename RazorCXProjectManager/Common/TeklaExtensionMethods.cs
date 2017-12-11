using System.Collections.Generic;
using System.Linq;
using RazorCXProjectManager.Beams;
using Tekla.Structures.Model;

namespace RazorCXProjectManager.Common
{
	public static class TeklaExtensionMethods
	{
		public static List<ModelObject> ToList(this ModelObjectEnumerator enumerator)
		{
			enumerator.SelectInstances = false;

			var modelObjects = new List<ModelObject>();
			while (enumerator.MoveNext())
			{
				var modelObject = enumerator.Current;
				if (modelObject == null) continue;
				modelObjects.Add(modelObject);
			}

			return modelObjects;
		}

		public static List<Beam> FilterForPartType(this IEnumerable<Beam> collection, PartTypeEnum partType)
		{
			var childParts = collection
				.AsParallel()
				.Where(b => b.GetFatherComponent() != null)
				.ToList();

			var nonChildParts = collection
				.AsParallel()
				.Where(b => b.GetFatherComponent() == null)
				.ToList();

			var assemblyId = -1;
			var partId = -1;
			var result = new List<Beam>();

			switch (partType)
			{
				case PartTypeEnum.MAIN_BEAMS:
					result = nonChildParts
						.Where(b =>
						{
							b.GetReportProperty("ASSEMBLY.MAINPART.ID", ref assemblyId);
							b.GetReportProperty("ID", ref partId);
							return assemblyId == partId;
						})
						.ToList();
					return result;
				case PartTypeEnum.SECONDARY_BEAMS:
					result = nonChildParts
						.Where(b =>
						{
							b.GetReportProperty("ASSEMBLY.MAINPART.ID", ref assemblyId);
							b.GetReportProperty("ID", ref partId);
							return assemblyId != partId;
						})
						.ToList();
					return result;
				case PartTypeEnum.CONNECTION_BEAMS:
					return childParts;

				default:
					return new List<Beam>();
			}
		}
	}
}
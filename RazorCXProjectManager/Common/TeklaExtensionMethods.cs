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

			switch (partType)
			{
				case PartTypeEnum.MAIN_BEAMS:
					return nonChildParts
						.Where(b => b.GetAssembly().GetMainPart().Identifier.ID == b.Identifier.ID)
						.ToList();
				case PartTypeEnum.SECONDARY_BEAMS:
					return nonChildParts
						.Where(b => b.GetFatherComponent() == null)
						.Where(b => b.GetAssembly().GetMainPart().Identifier.ID != b.Identifier.ID)
						.ToList();
				case PartTypeEnum.CONNECTION_BEAMS:
					return childParts;

				default:
					return new List<Beam>();
			}
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;

namespace RazorCXProjectManager
{
	public class OpenShopDrawing
	{
		private static DrawingHandler Handler => new DrawingHandler();

		public void Open(Tekla.Structures.Model.Part part)
		{
			//Check if numbering up to date
			if (!Operation.IsNumberingUpToDate(part))
				return;

			//Get drawings
			var drawingCollection = Handler.GetDrawings();
			if (drawingCollection.GetSize() < 1)
				return;

			//Get drawings by type
			var drawings = new List<Drawing>();
			while (drawingCollection.MoveNext())
			{
				drawingCollection.SelectInstances = false;
				drawings.Add(drawingCollection.Current);
			}

			var assemblyDrawings = new List<AssemblyDrawing>(drawings.OfType<AssemblyDrawing>());
			var singlePartDrawings = new List<SinglePartDrawing>(drawings.OfType<SinglePartDrawing>());

			//Get assembly mark
			var assemblyMark = string.Empty;
			part.GetReportProperty("ASSEMBLY_POS", ref assemblyMark);
			if (string.IsNullOrEmpty(assemblyMark))
				return;

			var singlePartDrawing = singlePartDrawings.AsParallel()
				.FirstOrDefault(s => GetDrawingUsableMark(s) == assemblyMark);

			var assemblyDrawing = assemblyDrawings.AsParallel()
				.FirstOrDefault(s => GetDrawingUsableMark(s) == assemblyMark);

			if (assemblyDrawing != null)
				Handler.SetActiveDrawing(assemblyDrawing);
			else if (singlePartDrawing != null)
				Handler.SetActiveDrawing(singlePartDrawing);
		}

		private static string GetDrawingUsableMark(Drawing drawing)
		{
			var mark = string.Empty;
			if (drawing is AssemblyDrawing)
			{
				var part =
					new Model().SelectModelObject(((AssemblyDrawing)drawing).AssemblyIdentifier);
				part?.GetReportProperty("ASSEMBLY_POS", ref mark);
				return mark;
			}
			if (drawing is SinglePartDrawing)
			{
				var part =
					new Model().SelectModelObject(((SinglePartDrawing)drawing).PartIdentifier);
				part?.GetReportProperty("ASSEMBLY_POS", ref mark);
				return mark;
			}
			return string.Empty;
		}
	}

}

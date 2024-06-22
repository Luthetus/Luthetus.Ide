namespace Luthetus.Ide.RazorLib.DotNetSolutions.Models.Internals;

public class SolutionVisualizationDrawing<TItem> : ISolutionVisualizationDrawing
{
	public TItem Item { get; set; }
	public SolutionVisualizationDrawingKind SolutionVisualizationDrawingKind { get; set; }
	public int CenterX { get; set; }
	public int CenterY { get; set; }
	public int Radius { get; set; }
	public string Fill { get; set; }
	public int RenderCycle { get; set; }
	public int RenderCycleSequence { get; set; }

	object ISolutionVisualizationDrawing.Item => Item;
}

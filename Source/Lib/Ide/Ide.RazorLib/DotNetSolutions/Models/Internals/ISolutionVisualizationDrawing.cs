namespace Luthetus.Ide.RazorLib.DotNetSolutions.Models.Internals;

public interface ISolutionVisualizationDrawing
{
	public object Item { get; }
	public SolutionVisualizationDrawingKind SolutionVisualizationDrawingKind { get; set; }
	public int CenterX { get; set; }
	public int CenterY { get; set; }
	public int Radius { get; set; }
	public string Fill { get; set; }
	
	/// <summary>
	/// Macro-filter for the order that things render.
	/// 0 is the first "render cycle", and anything with that value
	/// will render first.
	/// </summary>
	public int RenderCycle { get; set; }
	/// <summary>
	/// Micro-filter for the order that things render.
	/// 0 is the first "render cycle sequence", and anything with that value
	/// would have been the first in a given render cycle to have rendered.
	/// </summary>
	public int RenderCycleSequence { get; set; }
}

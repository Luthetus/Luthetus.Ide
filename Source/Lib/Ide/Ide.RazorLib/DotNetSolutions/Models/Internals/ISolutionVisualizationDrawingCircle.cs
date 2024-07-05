namespace Luthetus.Ide.RazorLib.DotNetSolutions.Models.Internals;

public interface ISolutionVisualizationDrawingCircle : ISolutionVisualizationDrawing
{
	public int CenterX { get; set; }
	public int CenterY { get; set; }
	public int Radius { get; set; }
	public string Fill { get; set; }
}

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Models;

public class SolutionVisualizationModel
{
	public int SvgWidth { get; set; } = 400;
	public int SvgHeight { get; set; } = 400;
	public int ViewBoxMinX { get; set; } = 0;
	public int ViewBoxMinY { get; set; } = 0;
	public int ViewBoxWidth { get; set; } = 100;
	public int ViewBoxHeight { get; set; } = 100;
}

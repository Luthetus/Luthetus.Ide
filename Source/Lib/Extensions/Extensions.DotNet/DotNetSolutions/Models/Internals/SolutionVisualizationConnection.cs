namespace Luthetus.Extensions.DotNet.DotNetSolutions.Models.Internals;

public class SolutionVisualizationConnection
{
	public SolutionVisualizationConnection(
		ISolutionVisualizationDrawing startSolutionVisualizationDrawing,
		ISolutionVisualizationDrawing endSolutionVisualizationDrawing)
	{
		StartSolutionVisualizationDrawing = startSolutionVisualizationDrawing;
		EndSolutionVisualizationDrawing = endSolutionVisualizationDrawing;
	}

	public ISolutionVisualizationDrawing StartSolutionVisualizationDrawing { get; }
	public ISolutionVisualizationDrawing EndSolutionVisualizationDrawing { get; }
}

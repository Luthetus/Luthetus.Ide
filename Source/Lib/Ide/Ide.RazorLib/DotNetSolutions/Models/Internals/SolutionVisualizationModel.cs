using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Models.Internals;

public class SolutionVisualizationModel
{
	private readonly Action _onStateChangedAction;

	public SolutionVisualizationModel(Action onStateChangedAction)
	{
		_onStateChangedAction = onStateChangedAction;
		Dimensions = new(_onStateChangedAction);
	}

	public SolutionVisualizationDimensions Dimensions { get; set; }
	public List<ISolutionVisualizationDrawing> SolutionVisualizationDrawingList { get; set; } = new();
	public List<List<ISolutionVisualizationDrawing>> SolutionVisualizationDrawingRenderCycleList { get; set; } = new();

	public SolutionVisualizationModel ShallowClone()
	{
		return new SolutionVisualizationModel(_onStateChangedAction)
		{
			Dimensions = Dimensions,
			SolutionVisualizationDrawingList = new(SolutionVisualizationDrawingList),
			SolutionVisualizationDrawingRenderCycleList = new(SolutionVisualizationDrawingRenderCycleList),
		};
	}
}

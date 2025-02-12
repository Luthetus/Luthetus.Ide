using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

public class TestExplorerService : ITestExplorerService
{  
	private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IDotNetSolutionService _dotNetSolutionService;

    public TestExplorerService(
		DotNetBackgroundTaskApi dotNetBackgroundTaskApi,
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		IDotNetSolutionService dotNetSolutionService)
	{
        _dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _dotNetSolutionService = dotNetSolutionService;
    }
    
    /// <summary>
    /// Each time the user opens the 'Test Explorer' panel,
    /// a check is done to see if the data being displayed
    /// is in sync with the user's selected .NET solution.
    ///
    /// If it is not in sync, then it starts discovering tests for each of the
    /// projects in the solution.
    ///
    /// But, if the user cancels this task, if they change panel tabs
    /// from the 'Test Explorer' to something else, when they return
    /// it will once again try to discover tests in all the projects for the solution.
    ///
    /// This is very annoying from a user perspective.
    /// So this field will track whether we've already started
    /// the task to discover tests in all the projects for the solution or not.
    ///
    /// This is fine because there is a button in the top left of the panel that
    /// has a 'refresh' icon and it will start this task if the
    /// user manually clicks it, (even if they cancelled the automatic invocation).
    /// </summary>
    private string _intentToDiscoverTestsInSolutionFilePath = string.Empty;
    
    private TestExplorerState _testExplorerState = new();
    
    public event Action? TestExplorerStateChanged;
    
    public TestExplorerState GetTestExplorerState() => _testExplorerState;

    public void ReduceWithAction(Func<TestExplorerState, TestExplorerState> withFunc)
    {
    	var inState = GetTestExplorerState();
    
        _testExplorerState = withFunc.Invoke(inState);
        
        TestExplorerStateChanged?.Invoke();
        return;
    }
    
    public void ReduceInitializeResizeHandleDimensionUnitAction(DimensionUnit dimensionUnit)
    {
    	var inState = GetTestExplorerState();
    
        if (dimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN)
        {
        	TestExplorerStateChanged?.Invoke();
        	return;
        }
        
        // TreeViewElementDimensions
        {
        	if (inState.TreeViewElementDimensions.WidthDimensionAttribute.DimensionUnitList is null)
        	{
        		TestExplorerStateChanged?.Invoke();
        		return;
        	}
        		
        	var existingDimensionUnit = inState.TreeViewElementDimensions.WidthDimensionAttribute.DimensionUnitList
        		.FirstOrDefault(x => x.Purpose == dimensionUnit.Purpose);
        		
            if (existingDimensionUnit.Purpose is not null)
            {
            	TestExplorerStateChanged?.Invoke();
        		return;
            }
        		
        	inState.TreeViewElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(dimensionUnit);
        }
        
        // DetailsElementDimensions
        {
        	if (inState.DetailsElementDimensions.WidthDimensionAttribute.DimensionUnitList is null)
        	{
        		TestExplorerStateChanged?.Invoke();
        		return;
        	}
        		
        	var existingDimensionUnit = inState.DetailsElementDimensions.WidthDimensionAttribute.DimensionUnitList
        		.FirstOrDefault(x => x.Purpose == dimensionUnit.Purpose);
        		
            if (existingDimensionUnit.Purpose is not null)
            {
            	TestExplorerStateChanged?.Invoke();
        		return;
            }
        		
        	inState.DetailsElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(dimensionUnit);
        }
        
        TestExplorerStateChanged?.Invoke();
        return;
    }
    
	public Task HandleDotNetSolutionStateStateHasChanged()
	{
        _dotNetBackgroundTaskApi.TestExplorer.Enqueue_ConstructTreeView();
		return Task.CompletedTask;
	}

	/// <summary>
    /// When the user interface for the test explorer is rendered,
    /// then dispatch this in order to start a task that will discover unit tests.
    /// </summary>
	public Task HandleUserInterfaceWasInitializedEffect()
	{
		var dotNetSolutionState = _dotNetSolutionService.GetDotNetSolutionState();
		var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

		if (dotNetSolutionModel is null)
			return Task.CompletedTask;

		var testExplorerState = GetTestExplorerState();

		if (dotNetSolutionModel.AbsolutePath.Value != testExplorerState.SolutionFilePath &&
			_intentToDiscoverTestsInSolutionFilePath != dotNetSolutionModel.AbsolutePath.Value)
		{
			_intentToDiscoverTestsInSolutionFilePath = dotNetSolutionModel.AbsolutePath.Value;
			HandleShouldDiscoverTestsEffect();
		}

		return Task.CompletedTask;
	}
	
	public Task HandleShouldDiscoverTestsEffect()
	{
        return _dotNetBackgroundTaskApi.TestExplorer.Task_DiscoverTests();
	}
}

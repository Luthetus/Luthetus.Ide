using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.States;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

public interface ITestExplorerService
{
	public event Action? TestExplorerStateChanged;
    
    public TestExplorerState GetTestExplorerState();

    public void ReduceWithAction(Func<TestExplorerState, TestExplorerState> withFunc);
    public void ReduceInitializeResizeHandleDimensionUnitAction(DimensionUnit dimensionUnit);
    
	public Task HandleDotNetSolutionStateStateHasChanged();

	/// <summary>
    /// When the user interface for the test explorer is rendered,
    /// then dispatch this in order to start a task that will discover unit tests.
    /// </summary>
	public Task HandleUserInterfaceWasInitializedEffect();
	
	public Task HandleShouldDiscoverTestsEffect();
}

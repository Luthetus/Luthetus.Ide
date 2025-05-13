using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

public interface ITestExplorerService
{
	public event Action? TestExplorerStateChanged;
    
    public TestExplorerState GetTestExplorerState();

    public void ReduceWithAction(Func<TestExplorerState, TestExplorerState> withFunc);
    public void ReduceInitializeResizeHandleDimensionUnitAction(DimensionUnit dimensionUnit);
    
	/// <summary>
    /// When the user interface for the test explorer is rendered,
    /// then dispatch this in order to start a task that will discover unit tests.
    /// </summary>
	public Task HandleUserInterfaceWasInitializedEffect();
	
	public Task HandleShouldDiscoverTestsEffect();
	
	public void Enqueue_ConstructTreeView();
    public void Enqueue_DiscoverTests();
    
    public void MoveNodeToCorrectBranch(TreeViewProjectTestModel treeViewProjectTestModel);
}

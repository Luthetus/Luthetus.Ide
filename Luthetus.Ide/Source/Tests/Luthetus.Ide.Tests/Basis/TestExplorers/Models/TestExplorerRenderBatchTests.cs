using Luthetus.Ide.RazorLib.TestExplorers.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Ide.Tests.Basis.TestExplorers.Models;

public class TestExplorerRenderBatchTests
{
    public TestExplorerRenderBatch(
        TestExplorerState testExplorerState,
        AppOptionsState appOptionsState,
        TreeViewContainer? treeViewContainer)
    {
        TestExplorerState = testExplorerState;
        AppOptionsState = appOptionsState;
        TreeViewContainer = treeViewContainer;
    }

    public TestExplorerState TestExplorerState { get; }
	public AppOptionsState AppOptionsState { get; }
	public TreeViewContainer? TreeViewContainer { get; }
}

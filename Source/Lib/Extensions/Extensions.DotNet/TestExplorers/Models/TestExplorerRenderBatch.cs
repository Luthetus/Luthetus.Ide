using Luthetus.Extensions.DotNet.TestExplorers.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

public class TestExplorerRenderBatch : ITestExplorerRenderBatch
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

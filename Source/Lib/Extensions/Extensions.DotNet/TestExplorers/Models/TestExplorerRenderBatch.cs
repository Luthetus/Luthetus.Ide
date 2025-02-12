using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;

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

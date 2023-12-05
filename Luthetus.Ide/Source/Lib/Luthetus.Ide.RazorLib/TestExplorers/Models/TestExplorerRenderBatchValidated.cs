using Luthetus.Ide.RazorLib.TestExplorers.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Models;

public class TestExplorerRenderBatchValidated : ITestExplorerRenderBatch
{
	public TestExplorerRenderBatchValidated(
		TestExplorerRenderBatch testExplorerRenderBatch)
	{
		TestExplorerState = testExplorerRenderBatch.TestExplorerState;
		AppOptionsState = testExplorerRenderBatch.AppOptionsState;
		TreeViewContainer = testExplorerRenderBatch.TreeViewContainer;
	}

	public TestExplorerState TestExplorerState { get; set; }
	public AppOptionsState AppOptionsState { get; set; }
	public TreeViewContainer TreeViewContainer { get; set; }
}

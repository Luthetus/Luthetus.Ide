using Luthetus.Ide.RazorLib.TestExplorers.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Ide.Tests.Basis.TestExplorers.Models;

public class TestExplorerRenderBatchValidatedTests
{
	public TestExplorerRenderBatchValidated(TestExplorerRenderBatch testExplorerRenderBatch)
	{
		TestExplorerState = testExplorerRenderBatch.TestExplorerState;
		AppOptionsState = testExplorerRenderBatch.AppOptionsState;

		TreeViewContainer = testExplorerRenderBatch.TreeViewContainer ??
			throw new NullReferenceException();
	}

	public TestExplorerState TestExplorerState { get; }
	public AppOptionsState AppOptionsState { get; }
	public TreeViewContainer TreeViewContainer { get; }
}

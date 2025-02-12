using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

public class TestExplorerRenderBatchValidated : ITestExplorerRenderBatch
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

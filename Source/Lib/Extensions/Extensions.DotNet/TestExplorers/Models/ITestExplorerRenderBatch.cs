using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

public interface ITestExplorerRenderBatch
{
	public TestExplorerState TestExplorerState { get; }
	public AppOptionsState AppOptionsState { get; }
	public TreeViewContainer? TreeViewContainer { get; }
}

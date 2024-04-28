using Luthetus.Ide.RazorLib.TestExplorers.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Models;

public interface ITestExplorerRenderBatch
{
	public TestExplorerState TestExplorerState { get; }
	public AppOptionsState AppOptionsState { get; }
	public TreeViewContainer? TreeViewContainer { get; }
}

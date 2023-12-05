using Luthetus.Ide.RazorLib.TestExplorers.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Models;

public interface ITestExplorerRenderBatch
{
	public TestExplorerState TestExplorerState { get; set; }
	public AppOptionsState AppOptionsState { get; set; }
	public TreeViewContainer TreeViewContainer { get; set; }
}

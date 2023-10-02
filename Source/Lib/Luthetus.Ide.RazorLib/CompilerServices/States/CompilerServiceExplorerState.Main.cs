using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.CompilerServices.Models;

namespace Luthetus.Ide.RazorLib.CompilerServices.States;

[FeatureState]
public partial class CompilerServiceExplorerState
{
    public static readonly Key<TreeViewContainer> TreeViewCompilerServiceExplorerContentStateKey = Key<TreeViewContainer>.NewKey();

    public CompilerServiceExplorerState()
    {
        Model = new CompilerServiceExplorerModel();
    }

    public CompilerServiceExplorerState(
        CompilerServiceExplorerModel model)
    {
        Model = model;
    }

    public CompilerServiceExplorerModel Model { get; }
}
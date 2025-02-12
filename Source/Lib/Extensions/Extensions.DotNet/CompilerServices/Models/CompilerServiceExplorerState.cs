using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Extensions.DotNet.CompilerServices.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Models;

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
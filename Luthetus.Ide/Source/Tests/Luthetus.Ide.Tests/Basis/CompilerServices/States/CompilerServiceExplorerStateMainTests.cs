using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.CompilerServices.Models;

namespace Luthetus.Ide.Tests.Basis.CompilerServices.States;

public class CompilerServiceExplorerStateMainTests
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
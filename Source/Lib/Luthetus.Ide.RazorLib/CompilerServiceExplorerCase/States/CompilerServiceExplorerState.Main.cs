using Fluxor;
using Luthetus.Common.RazorLib.KeyCase;
using Luthetus.Common.RazorLib.TabCase.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Models;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Viewables;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;

[FeatureState]
public partial class CompilerServiceExplorerState
{
    public static readonly Key<TreeViewState> TreeViewCompilerServiceExplorerContentStateKey = Key<TreeViewState>.NewKey();
    public static readonly Key<TabGroup> TabGroupKey = Key<TabGroup>.NewKey();

    public CompilerServiceExplorerState()
    {
        Model = new CompilerServiceExplorerModel();
        GraphicalView = new CompilerServiceExplorerGraphicalScene();
        ReflectionView = new CompilerServiceExplorerReflectionScene();
    }

    public CompilerServiceExplorerState(
        CompilerServiceExplorerModel model,
        CompilerServiceExplorerGraphicalScene graphicalView,
        CompilerServiceExplorerReflectionScene reflectionView)
    {
        Model = model;
        GraphicalView = graphicalView;
        ReflectionView = reflectionView;
    }

    public CompilerServiceExplorerModel Model { get; }
    public CompilerServiceExplorerGraphicalScene GraphicalView { get; }
    public CompilerServiceExplorerReflectionScene ReflectionView { get; }
}
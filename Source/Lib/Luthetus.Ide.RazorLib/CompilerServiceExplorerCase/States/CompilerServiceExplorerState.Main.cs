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
        GraphicalView = new CompilerServiceExplorerGraphicalViewable();
        ReflectionView = new CompilerServiceExplorerReflectionViewable();
    }

    public CompilerServiceExplorerState(
        CompilerServiceExplorerModel model,
        CompilerServiceExplorerGraphicalViewable graphicalView,
        CompilerServiceExplorerReflectionViewable reflectionView)
    {
        Model = model;
        GraphicalView = graphicalView;
        ReflectionView = reflectionView;
    }

    public CompilerServiceExplorerModel Model { get; }
    public CompilerServiceExplorerGraphicalViewable GraphicalView { get; }
    public CompilerServiceExplorerReflectionViewable ReflectionView { get; }
}
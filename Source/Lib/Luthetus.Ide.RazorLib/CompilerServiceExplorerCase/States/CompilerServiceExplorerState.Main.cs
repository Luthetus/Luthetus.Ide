using Fluxor;
using Luthetus.Common.RazorLib.KeyCase.Models;
using Luthetus.Common.RazorLib.TabCase.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Models;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Scenes;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;

[FeatureState]
public partial class CompilerServiceExplorerState
{
    public static readonly Key<TreeViewContainer> TreeViewCompilerServiceExplorerContentStateKey = Key<TreeViewContainer>.NewKey();
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
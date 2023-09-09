using Fluxor;
using Luthetus.Common.RazorLib.TabGroups;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase.InnerDetails;

namespace Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase;

[FeatureState]
public partial class CompilerServiceExplorerState
{
    public static readonly TreeViewStateKey TreeViewCompilerServiceExplorerContentStateKey = TreeViewStateKey.NewTreeViewStateKey();
    public static readonly TabGroupKey TabGroupKey = TabGroupKey.NewTabGroupKey();

    private CompilerServiceExplorerState()
    {
        Model = new CompilerServiceExplorerModel();
        GraphicalView = new CompilerServiceExplorerGraphicalView();
        ReflectionView = new CompilerServiceExplorerReflectionView();
    }

    private CompilerServiceExplorerState(
        CompilerServiceExplorerModel model,
        CompilerServiceExplorerGraphicalView graphicalView,
        CompilerServiceExplorerReflectionView reflectionView)
    {
        Model = model;
        GraphicalView = graphicalView;
        ReflectionView = reflectionView;
    }

    public CompilerServiceExplorerModel Model { get; }
    public CompilerServiceExplorerGraphicalView GraphicalView { get; }
    public CompilerServiceExplorerReflectionView ReflectionView { get; }
}
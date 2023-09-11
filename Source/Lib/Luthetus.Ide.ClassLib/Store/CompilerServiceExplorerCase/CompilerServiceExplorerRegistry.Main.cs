using Fluxor;
using Luthetus.Common.RazorLib.TabGroups;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase.InnerDetails;

namespace Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase;

[FeatureState]
public partial class CompilerServiceExplorerRegistry
{
    public static readonly TreeViewStateKey TreeViewCompilerServiceExplorerContentStateKey = TreeViewStateKey.NewKey();
    public static readonly TabGroupKey TabGroupKey = TabGroupKey.NewTabGroupKey();

    private CompilerServiceExplorerRegistry()
    {
        Model = new CompilerServiceExplorerModel();
        GraphicalView = new CompilerServiceExplorerGraphicalView();
        ReflectionView = new CompilerServiceExplorerReflectionView();
    }

    private CompilerServiceExplorerRegistry(
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
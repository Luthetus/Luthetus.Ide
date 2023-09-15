using Fluxor;
using Luthetus.Common.RazorLib.TabCase;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Models;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Viewables;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;

[FeatureState]
public partial class CompilerServiceExplorerRegistry
{
    public static readonly TreeViewStateKey TreeViewCompilerServiceExplorerContentStateKey = TreeViewStateKey.NewKey();
    public static readonly TabGroupKey TabGroupKey = TabGroupKey.NewKey();

    private CompilerServiceExplorerRegistry()
    {
        Model = new CompilerServiceExplorerModel();
        GraphicalView = new CompilerServiceExplorerGraphicalViewable();
        ReflectionView = new CompilerServiceExplorerReflectionViewable();
    }

    private CompilerServiceExplorerRegistry(
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
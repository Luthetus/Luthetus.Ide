using Fluxor;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Ide.RazorLib.EditorCase;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.Models;

public class SolutionExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly IDispatcher _dispatcher;

    public SolutionExplorerTreeViewMouseEventHandler(
        IDispatcher dispatcher,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
        _dispatcher = dispatcher;
    }

    public override void OnDoubleClick(ITreeViewCommandParameter treeViewCommandParameter)
    {
        base.OnDoubleClick(treeViewCommandParameter);

        if (treeViewCommandParameter.TargetNode is not TreeViewNamespacePath treeViewNamespacePath)
            return;

        _dispatcher.Dispatch(new EditorRegistry.OpenInEditorAction(
            treeViewNamespacePath.Item.AbsolutePath,
            true));
    }
}
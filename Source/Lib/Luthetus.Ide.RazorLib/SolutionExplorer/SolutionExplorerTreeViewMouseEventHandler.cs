using Fluxor;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;

namespace Luthetus.Ide.RazorLib.SolutionExplorer;

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

        _dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            treeViewNamespacePath.Item.AbsoluteFilePath,
            true));
    }
}
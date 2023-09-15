using Fluxor;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Ide.RazorLib.EditorCase;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;

namespace Luthetus.Ide.RazorLib.FolderExplorerCase.Models;

public class FolderExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly IDispatcher _dispatcher;

    public FolderExplorerTreeViewMouseEventHandler(
        IDispatcher dispatcher,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
        _dispatcher = dispatcher;
    }

    public override void OnDoubleClick(ITreeViewCommandParameter treeViewCommandParameter)
    {
        base.OnDoubleClick(treeViewCommandParameter);

        if (treeViewCommandParameter.TargetNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        _dispatcher.Dispatch(new EditorRegistry.OpenInEditorAction(
            treeViewAbsolutePath.Item,
            true));
    }
}
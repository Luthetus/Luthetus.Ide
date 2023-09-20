using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Ide.RazorLib.EditorCase.States;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;

namespace Luthetus.Ide.RazorLib.FolderExplorerCase.Models;

public class FolderExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly EditorSync _editorSync;

    public FolderExplorerTreeViewMouseEventHandler(
        EditorSync editorSync,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
        _editorSync = editorSync;
    }

    public override void OnDoubleClick(TreeViewCommandParameter treeViewCommandParameter)
    {
        base.OnDoubleClick(treeViewCommandParameter);

        if (treeViewCommandParameter.TargetNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        _editorSync.Dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            _editorSync,
            treeViewAbsolutePath.Item,
            true));
    }
}
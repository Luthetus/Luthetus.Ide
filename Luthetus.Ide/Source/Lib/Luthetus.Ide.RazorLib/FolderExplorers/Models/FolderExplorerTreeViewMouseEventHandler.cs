using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.RazorLib.FolderExplorers.Models;

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

    public override Task OnDoubleClickAsync(TreeViewCommandArgs commandParameter)
    {
        base.OnDoubleClickAsync(commandParameter);

        if (commandParameter.TargetNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return Task.CompletedTask;

        _editorSync.OpenInEditor(treeViewAbsolutePath.Item, true);
        return Task.CompletedTask;
    }
}
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Ide.RazorLib.EditorCase.States;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Models;

public class CompilerServiceExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly EditorSync _editorSync;

    public CompilerServiceExplorerTreeViewMouseEventHandler(
        EditorSync editorSync,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
        _editorSync = editorSync;
    }

    public override Task OnDoubleClickAsync(TreeViewCommandParameter treeViewCommandParameter)
    {
        base.OnDoubleClickAsync(treeViewCommandParameter);

        if (treeViewCommandParameter.TargetNode is not TreeViewNamespacePath treeViewNamespacePath)
            return Task.CompletedTask;

        _editorSync.OpenInEditor(treeViewNamespacePath.Item.AbsolutePath, true);
        return Task.CompletedTask;
    }
}
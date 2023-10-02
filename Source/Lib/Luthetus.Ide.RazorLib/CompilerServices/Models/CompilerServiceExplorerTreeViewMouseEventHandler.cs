using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.RazorLib.CompilerServices.Models;

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

    public override Task OnDoubleClickAsync(TreeViewCommandParameter commandParameter)
    {
        base.OnDoubleClickAsync(commandParameter);

        if (commandParameter.TargetNode is not TreeViewNamespacePath treeViewNamespacePath)
            return Task.CompletedTask;

        _editorSync.OpenInEditor(treeViewNamespacePath.Item.AbsolutePath, true);
        return Task.CompletedTask;
    }
}
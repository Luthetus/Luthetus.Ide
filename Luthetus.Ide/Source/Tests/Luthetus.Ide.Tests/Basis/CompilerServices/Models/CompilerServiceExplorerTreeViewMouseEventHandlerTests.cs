using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.Tests.Basis.CompilerServices.Models;

public class CompilerServiceExplorerTreeViewMouseEventHandlerTests
{
    private readonly EditorSync _editorSync;

    public CompilerServiceExplorerTreeViewMouseEventHandler(
        EditorSync editorSync,
        ITreeViewService treeViewService,
		IBackgroundTaskService backgroundTaskService)
        : base(treeViewService, backgroundTaskService)
    {
        _editorSync = editorSync;
    }

    public override Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
    {
        base.OnDoubleClickAsync(commandArgs);

        if (commandArgs.TargetNode is not TreeViewNamespacePath treeViewNamespacePath)
            return Task.CompletedTask;

        _editorSync.OpenInEditor(treeViewNamespacePath.Item.AbsolutePath, true);
        return Task.CompletedTask;
    }
}
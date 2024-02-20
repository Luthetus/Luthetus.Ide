using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.Tests.Basis.FolderExplorers.Models;

public class FolderExplorerTreeViewMouseEventHandlerTests
{
    private readonly EditorSync _editorSync;

    public FolderExplorerTreeViewMouseEventHandler(
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

        if (commandArgs.TargetNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return Task.CompletedTask;

        _editorSync.OpenInEditor(treeViewAbsolutePath.Item, true);
        return Task.CompletedTask;
    }
}
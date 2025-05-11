using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Models;

public class InputFileTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly IInputFileService _inputFileService;
    private readonly Func<AbsolutePath, Task> _setInputFileContentTreeViewRootFunc;

    public InputFileTreeViewMouseEventHandler(
        ITreeViewService treeViewService,
        IInputFileService inputFileService,
        Func<AbsolutePath, Task> setInputFileContentTreeViewRootFunc,
		BackgroundTaskService backgroundTaskService)
        : base(treeViewService, backgroundTaskService)
    {
        _inputFileService = inputFileService;
        _setInputFileContentTreeViewRootFunc = setInputFileContentTreeViewRootFunc;
    }

    protected override void OnClick(TreeViewCommandArgs commandArgs)
    {
        base.OnClick(commandArgs);

        if (commandArgs.NodeThatReceivedMouseEvent is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        _inputFileService.SetSelectedTreeViewModel(treeViewAbsolutePath);
    }

    public override Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
    {
        base.OnDoubleClickAsync(commandArgs);

        if (commandArgs.NodeThatReceivedMouseEvent is not TreeViewAbsolutePath treeViewAbsolutePath)
            return Task.CompletedTask;

        _setInputFileContentTreeViewRootFunc.Invoke(treeViewAbsolutePath.Item);
        return Task.CompletedTask;
    }
}
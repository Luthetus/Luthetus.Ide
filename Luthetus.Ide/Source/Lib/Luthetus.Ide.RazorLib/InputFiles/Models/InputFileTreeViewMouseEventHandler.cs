using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Models;

public class InputFileTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly IDispatcher _dispatcher;
    private readonly Func<IAbsolutePath, Task> _setInputFileContentTreeViewRootFunc;

    public InputFileTreeViewMouseEventHandler(
        ITreeViewService treeViewService,
        IDispatcher dispatcher,
        Func<IAbsolutePath, Task> setInputFileContentTreeViewRootFunc)
        : base(treeViewService)
    {
        _dispatcher = dispatcher;
        _setInputFileContentTreeViewRootFunc = setInputFileContentTreeViewRootFunc;
    }

    public override void OnClick(TreeViewCommandArgs commandParameter)
    {
        base.OnClick(commandParameter);

        if (commandParameter.TargetNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        var setSelectedTreeViewModelAction = new InputFileState.SetSelectedTreeViewModelAction(treeViewAbsolutePath);

        _dispatcher.Dispatch(setSelectedTreeViewModelAction);
    }

    public override Task OnDoubleClickAsync(TreeViewCommandArgs commandParameter)
    {
        base.OnDoubleClickAsync(commandParameter);

        if (commandParameter.TargetNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return Task.CompletedTask;

        _setInputFileContentTreeViewRootFunc.Invoke(treeViewAbsolutePath.Item);
        return Task.CompletedTask;
    }
}
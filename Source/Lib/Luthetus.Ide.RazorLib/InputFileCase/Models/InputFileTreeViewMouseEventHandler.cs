using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Ide.RazorLib.InputFileCase.States;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;

namespace Luthetus.Ide.RazorLib.InputFileCase.Models;

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

    public override void OnClick(ITreeViewCommandParameter treeViewCommandParameter)
    {
        base.OnClick(treeViewCommandParameter);

        if (treeViewCommandParameter.TargetNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        var setSelectedTreeViewModelAction = new InputFileState.SetSelectedTreeViewModelAction(
            treeViewAbsolutePath);

        _dispatcher.Dispatch(setSelectedTreeViewModelAction);
    }

    public override void OnDoubleClick(ITreeViewCommandParameter treeViewCommandParameter)
    {
        base.OnDoubleClick(treeViewCommandParameter);

        if (treeViewCommandParameter.TargetNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        _setInputFileContentTreeViewRootFunc.Invoke(treeViewAbsolutePath.Item);
    }
}
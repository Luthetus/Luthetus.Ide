using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Ide.ClassLib.Store.InputFileCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;

namespace Luthetus.Ide.RazorLib.InputFile.Classes;

public class InputFileTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly IDispatcher _dispatcher;
    private readonly Func<IAbsoluteFilePath, Task> _setInputFileContentTreeViewRootFunc;

    public InputFileTreeViewMouseEventHandler(
        ITreeViewService treeViewService,
        IDispatcher dispatcher,
        Func<IAbsoluteFilePath, Task> setInputFileContentTreeViewRootFunc)
        : base(treeViewService)
    {
        _dispatcher = dispatcher;
        _setInputFileContentTreeViewRootFunc = setInputFileContentTreeViewRootFunc;
    }

    public override void OnClick(ITreeViewCommandParameter treeViewCommandParameter)
    {
        base.OnClick(treeViewCommandParameter);

        if (treeViewCommandParameter.TargetNode is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)
            return;

        var setSelectedTreeViewModelAction = new InputFileRegistry.SetSelectedTreeViewModelAction(
            treeViewAbsoluteFilePath);

        _dispatcher.Dispatch(setSelectedTreeViewModelAction);
    }

    public override void OnDoubleClick(ITreeViewCommandParameter treeViewCommandParameter)
    {
        base.OnDoubleClick(treeViewCommandParameter);

        if (treeViewCommandParameter.TargetNode is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)
            return;

        _setInputFileContentTreeViewRootFunc.Invoke(treeViewAbsoluteFilePath.Item);
    }
}
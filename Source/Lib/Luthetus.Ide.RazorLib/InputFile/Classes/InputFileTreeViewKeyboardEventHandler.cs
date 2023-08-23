using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.Store.InputFileCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;

namespace Luthetus.Ide.RazorLib.InputFile.Classes;

public class InputFileTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly IState<InputFileState> _inputFileStateWrap;
    private readonly IDispatcher _dispatcher;
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly Func<IAbsoluteFilePath, Task> _setInputFileContentTreeViewRootFunc;
    private readonly Func<Task> _focusSearchInputElementFunc;
    private readonly Func<List<(TreeViewStateKey treeViewStateKey, TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)>> _getSearchMatchTuplesFunc;
    private readonly ICommonBackgroundTaskQueue _commonBackgroundTaskQueue;

    public InputFileTreeViewKeyboardEventHandler(
        ITreeViewService treeViewService,
        IState<InputFileState> inputFileStateWrap,
        IDispatcher dispatcher,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        Func<IAbsoluteFilePath, Task> setInputFileContentTreeViewRootFunc,
        Func<Task> focusSearchInputElementFunc,
        Func<List<(TreeViewStateKey treeViewStateKey, TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)>> getSearchMatchTuplesFunc,
        ICommonBackgroundTaskQueue commonBackgroundTaskQueue)
        : base(treeViewService)
    {
        _inputFileStateWrap = inputFileStateWrap;
        _dispatcher = dispatcher;
        _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _setInputFileContentTreeViewRootFunc = setInputFileContentTreeViewRootFunc;
        _focusSearchInputElementFunc = focusSearchInputElementFunc;
        _getSearchMatchTuplesFunc = getSearchMatchTuplesFunc;
        _commonBackgroundTaskQueue = commonBackgroundTaskQueue;
    }

    public override async Task<bool> OnKeyDownAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        if (treeViewCommandParameter.KeyboardEventArgs is null)
            return false;

        switch (treeViewCommandParameter.KeyboardEventArgs.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
                await SetInputFileContentTreeViewRootAsync(treeViewCommandParameter);
                return true;
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                await SetSelectedTreeViewModelAsync(treeViewCommandParameter);
                return true;
        }

        switch (treeViewCommandParameter.KeyboardEventArgs.Key)
        {
            // Tried to have { "Ctrl" + "f" } => MoveFocusToSearchBar
            // however, the webview was ending up taking over
            // and displaying its search bar with focus being set to it.
            //
            // Doing preventDefault just for this one case would be a can of
            // worms as JSInterop is needed, as well a custom Blazor event.
            case "/":
            case "?":
                await MoveFocusToSearchBarAsync(treeViewCommandParameter);
                return true;
                // TODO: Add move to next match and move to previous match
                //
                // case "*":
                //     treeViewCommand = new TreeViewCommand(SetNextMatchAsActiveTreeViewNode);
                //     return true;
                // case "#":
                //     treeViewCommand = new TreeViewCommand(SetPreviousMatchAsActiveTreeViewNode);
                //     return true;
        }

        if (treeViewCommandParameter.KeyboardEventArgs.AltKey)
        {
            var wasMappedToAnAction = await AltModifiedKeymapAsync(treeViewCommandParameter);

            if (wasMappedToAnAction)
                return wasMappedToAnAction;
        }

        return await base.OnKeyDownAsync(treeViewCommandParameter);
    }

    private async Task<bool> AltModifiedKeymapAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        if (treeViewCommandParameter.KeyboardEventArgs is null)
            return false;

        switch (treeViewCommandParameter.KeyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                await HandleBackButtonOnClickAsync(treeViewCommandParameter);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                await HandleUpwardButtonOnClick(treeViewCommandParameter);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
                await HandleForwardButtonOnClick(treeViewCommandParameter);
                break;
            case "r":
                await HandleRefreshButtonOnClick(treeViewCommandParameter);
                break;
        }

        return false;
    }

    private async Task SetInputFileContentTreeViewRootAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        var treeViewAbsoluteFilePath = activeNode as TreeViewAbsoluteFilePath;

        if (treeViewAbsoluteFilePath is null)
            return;

        await _setInputFileContentTreeViewRootFunc.Invoke(
            treeViewAbsoluteFilePath.Item);
    }

    private async Task HandleBackButtonOnClickAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _dispatcher.Dispatch(new InputFileState.MoveBackwardsInHistoryAction());

        await ChangeContentRootToOpenedTreeView(_inputFileStateWrap.Value);
    }

    private async Task HandleForwardButtonOnClick(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _dispatcher.Dispatch(new InputFileState.MoveForwardsInHistoryAction());

        await ChangeContentRootToOpenedTreeView(_inputFileStateWrap.Value);
    }

    private async Task HandleUpwardButtonOnClick(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _dispatcher.Dispatch(new InputFileState.OpenParentDirectoryAction(
            _luthetusIdeComponentRenderers,
            _luthetusCommonComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            _commonBackgroundTaskQueue));

        await ChangeContentRootToOpenedTreeView(_inputFileStateWrap.Value);
    }

    private async Task HandleRefreshButtonOnClick(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _dispatcher.Dispatch(
            new InputFileState.RefreshCurrentSelectionAction(_commonBackgroundTaskQueue));

        await ChangeContentRootToOpenedTreeView(_inputFileStateWrap.Value);
    }

    private async Task ChangeContentRootToOpenedTreeView(
        InputFileState inputFileState)
    {
        var openedTreeView = inputFileState.GetOpenedTreeView();

        if (openedTreeView.Item is not null)
            await _setInputFileContentTreeViewRootFunc.Invoke(openedTreeView.Item);
    }

    private Task SetSelectedTreeViewModelAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        var treeViewAbsoluteFilePath = activeNode as TreeViewAbsoluteFilePath;

        if (treeViewAbsoluteFilePath is null)
            return Task.CompletedTask;

        var setSelectedTreeViewModelAction =
            new InputFileState.SetSelectedTreeViewModelAction(
                treeViewAbsoluteFilePath);

        _dispatcher.Dispatch(setSelectedTreeViewModelAction);

        return Task.CompletedTask;
    }

    private async Task MoveFocusToSearchBarAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        await _focusSearchInputElementFunc.Invoke();
    }
}
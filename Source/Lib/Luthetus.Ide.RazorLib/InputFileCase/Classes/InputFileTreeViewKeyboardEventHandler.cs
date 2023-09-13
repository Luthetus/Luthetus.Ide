using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.Events;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.RazorLib.InputFileCase;
using Luthetus.Ide.RazorLib.ComponentRenderersCase;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase;

namespace Luthetus.Ide.RazorLib.InputFileCase.Classes;

public class InputFileTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly IState<InputFileRegistry> _inputFileStateWrap;
    private readonly IDispatcher _dispatcher;
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly Func<IAbsolutePath, Task> _setInputFileContentTreeViewRootFunc;
    private readonly Func<Task> _focusSearchInputElementFunc;
    private readonly Func<List<(TreeViewStateKey treeViewStateKey, TreeViewAbsolutePath treeViewAbsolutePath)>> _getSearchMatchTuplesFunc;
    private readonly IBackgroundTaskService _backgroundTaskService;

    public InputFileTreeViewKeyboardEventHandler(
        ITreeViewService treeViewService,
        IState<InputFileRegistry> inputFileStateWrap,
        IDispatcher dispatcher,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        Func<IAbsolutePath, Task> setInputFileContentTreeViewRootFunc,
        Func<Task> focusSearchInputElementFunc,
        Func<List<(TreeViewStateKey treeViewStateKey, TreeViewAbsolutePath treeViewAbsolutePath)>> getSearchMatchTuplesFunc,
        IBackgroundTaskService backgroundTaskService)
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
        _backgroundTaskService = backgroundTaskService;
    }

    public override void OnKeyDown(ITreeViewCommandParameter treeViewCommandParameter)
    {
        base.OnKeyDown(treeViewCommandParameter);

        if (treeViewCommandParameter.KeyboardEventArgs is null)
            return;

        switch (treeViewCommandParameter.KeyboardEventArgs.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
                SetInputFileContentTreeViewRoot(treeViewCommandParameter);
                return;
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                SetSelectedTreeViewModel(treeViewCommandParameter);
                return;
        }

        switch (treeViewCommandParameter.KeyboardEventArgs.Key)
        {
            // Tried to have { "Ctrl" + "f" } => MoveFocusToSearchBar however, the webview was ending up taking over
            // and displaying its search bar with focus being set to it.
            //
            // Doing preventDefault just for this one case would be a can of worms as JSInterop is needed, as well a custom Blazor event.
            case "/":
            case "?":
                MoveFocusToSearchBar(treeViewCommandParameter);
                return;
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
            AltModifiedKeymap(treeViewCommandParameter);
            return;
        }
    }

    private void AltModifiedKeymap(ITreeViewCommandParameter treeViewCommandParameter)
    {
        if (treeViewCommandParameter.KeyboardEventArgs is null)
            return;

        switch (treeViewCommandParameter.KeyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                HandleBackButtonOnClick(treeViewCommandParameter);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                HandleUpwardButtonOnClick(treeViewCommandParameter);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
                HandleForwardButtonOnClick(treeViewCommandParameter);
                break;
            case "r":
                HandleRefreshButtonOnClick(treeViewCommandParameter);
                break;
        }
    }

    private void SetInputFileContentTreeViewRoot(ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        _setInputFileContentTreeViewRootFunc.Invoke(treeViewAbsolutePath.Item);
    }

    private void HandleBackButtonOnClick(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _dispatcher.Dispatch(new InputFileRegistry.MoveBackwardsInHistoryAction());

        ChangeContentRootToOpenedTreeView(_inputFileStateWrap.Value);
    }

    private void HandleForwardButtonOnClick(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _dispatcher.Dispatch(new InputFileRegistry.MoveForwardsInHistoryAction());

        ChangeContentRootToOpenedTreeView(_inputFileStateWrap.Value);
    }

    private void HandleUpwardButtonOnClick(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _dispatcher.Dispatch(new InputFileRegistry.OpenParentDirectoryAction(
            _luthetusIdeComponentRenderers,
            _luthetusCommonComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            _backgroundTaskService));

        ChangeContentRootToOpenedTreeView(_inputFileStateWrap.Value);
    }

    private void HandleRefreshButtonOnClick(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _dispatcher.Dispatch(new InputFileRegistry.RefreshCurrentSelectionAction(
            _backgroundTaskService));

        ChangeContentRootToOpenedTreeView(_inputFileStateWrap.Value);
    }

    private void ChangeContentRootToOpenedTreeView(InputFileRegistry inputFileState)
    {
        var openedTreeView = inputFileState.GetOpenedTreeView();

        if (openedTreeView.Item is not null)
            _setInputFileContentTreeViewRootFunc.Invoke(openedTreeView.Item);
    }

    private void SetSelectedTreeViewModel(ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        var treeViewAbsolutePath = activeNode as TreeViewAbsolutePath;

        if (treeViewAbsolutePath is null)
            return;

        var setSelectedTreeViewModelAction =
            new InputFileRegistry.SetSelectedTreeViewModelAction(
                treeViewAbsolutePath);

        _dispatcher.Dispatch(setSelectedTreeViewModelAction);

        return;
    }

    private void MoveFocusToSearchBar(ITreeViewCommandParameter treeViewCommandParameter)
    {
        Task.Run(async () => await _focusSearchInputElementFunc.Invoke());
    }
}
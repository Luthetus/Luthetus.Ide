using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Models;

public class InputFileTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly IInputFileService _inputFileService;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly Func<AbsolutePath, Task> _setInputFileContentTreeViewRootFunc;
    private readonly Func<Task> _focusSearchInputElementFunc;
    private readonly Func<List<(Key<TreeViewContainer> treeViewStateKey, TreeViewAbsolutePath treeViewAbsolutePath)>> _getSearchMatchTuplesFunc;
    private readonly BackgroundTaskService _backgroundTaskService;

    public InputFileTreeViewKeyboardEventHandler(
	        ITreeViewService treeViewService,
	        IInputFileService inputFileService,
	        IIdeComponentRenderers ideComponentRenderers,
	        ICommonComponentRenderers commonComponentRenderers,
	        IFileSystemProvider fileSystemProvider,
	        IEnvironmentProvider environmentProvider,
	        Func<AbsolutePath, Task> setInputFileContentTreeViewRootFunc,
	        Func<Task> focusSearchInputElementFunc,
	        Func<List<(Key<TreeViewContainer> treeViewStateKey, TreeViewAbsolutePath treeViewAbsolutePath)>> getSearchMatchTuplesFunc,
	        BackgroundTaskService backgroundTaskService)
        : base(treeViewService, backgroundTaskService)
    {
        _inputFileService = inputFileService;
        _ideComponentRenderers = ideComponentRenderers;
        _commonComponentRenderers = commonComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _setInputFileContentTreeViewRootFunc = setInputFileContentTreeViewRootFunc;
        _focusSearchInputElementFunc = focusSearchInputElementFunc;
        _getSearchMatchTuplesFunc = getSearchMatchTuplesFunc;
        _backgroundTaskService = backgroundTaskService;
    }

    public override Task OnKeyDownAsync(TreeViewCommandArgs commandArgs)
    {
        base.OnKeyDownAsync(commandArgs);

        if (commandArgs.KeyboardEventArgs is null)
            return Task.CompletedTask;

        switch (commandArgs.KeyboardEventArgs.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
                SetInputFileContentTreeViewRoot(commandArgs);
                return Task.CompletedTask;
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                SetSelectedTreeViewModel(commandArgs);
                return Task.CompletedTask;
        }

        switch (commandArgs.KeyboardEventArgs.Key)
        {
            // Tried to have { "Ctrl" + "f" } => MoveFocusToSearchBar however, the webview was ending up taking over
            // and displaying its search bar with focus being set to it.
            //
            // Doing preventDefault just for this one case would be a can of worms as JSInterop is needed, as well a custom Blazor event.
            case "/":
            case "?":
                MoveFocusToSearchBar(commandArgs);
                return Task.CompletedTask;
                // TODO: Add move to next match and move to previous match
                //
                // case "*":
                //     treeViewCommand = new TreeViewCommand(SetNextMatchAsActiveTreeViewNode);
                //     return Task.CompletedTask true;
                // case "#":
                //     treeViewCommand = new TreeViewCommand(SetPreviousMatchAsActiveTreeViewNode);
                //     return Task.CompletedTask true;
        }

        if (commandArgs.KeyboardEventArgs.AltKey)
        {
            AltModifiedKeymap(commandArgs);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    private void AltModifiedKeymap(TreeViewCommandArgs commandArgs)
    {
        if (commandArgs.KeyboardEventArgs is null)
            return;

        switch (commandArgs.KeyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                HandleBackButtonOnClick(commandArgs);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                HandleUpwardButtonOnClick(commandArgs);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
                HandleForwardButtonOnClick(commandArgs);
                break;
            case "r":
                HandleRefreshButtonOnClick(commandArgs);
                break;
        }
    }

    private void SetInputFileContentTreeViewRoot(TreeViewCommandArgs commandArgs)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;

        if (activeNode is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        _setInputFileContentTreeViewRootFunc.Invoke(treeViewAbsolutePath.Item);
    }

    private void HandleBackButtonOnClick(TreeViewCommandArgs commandArgs)
    {
        _inputFileService.MoveBackwardsInHistory();
        ChangeContentRootToOpenedTreeView(_inputFileService.GetInputFileState());
    }

    private void HandleForwardButtonOnClick(TreeViewCommandArgs commandArgs)
    {
        _inputFileService.MoveForwardsInHistory();
        ChangeContentRootToOpenedTreeView(_inputFileService.GetInputFileState());
    }

    private void HandleUpwardButtonOnClick(TreeViewCommandArgs commandArgs)
    {
        _inputFileService.OpenParentDirectory(
            _ideComponentRenderers,
            _commonComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            _backgroundTaskService,
            parentDirectoryTreeViewModel: null);

        ChangeContentRootToOpenedTreeView(_inputFileService.GetInputFileState());
    }

    private void HandleRefreshButtonOnClick(TreeViewCommandArgs commandArgs)
    {
        _inputFileService.RefreshCurrentSelection(_backgroundTaskService, currentSelection: null);
        ChangeContentRootToOpenedTreeView(_inputFileService.GetInputFileState());
    }

    private void ChangeContentRootToOpenedTreeView(InputFileState inputFileState)
    {
        var openedTreeView = inputFileState.GetOpenedTreeView();

        if (openedTreeView?.Item is not null)
            _setInputFileContentTreeViewRootFunc.Invoke(openedTreeView.Item);
    }

    private void SetSelectedTreeViewModel(TreeViewCommandArgs commandArgs)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;
        var treeViewAbsolutePath = activeNode as TreeViewAbsolutePath;

        if (treeViewAbsolutePath is null)
            return;

        _inputFileService.SetSelectedTreeViewModel(treeViewAbsolutePath);
        return;
    }

    private void MoveFocusToSearchBar(TreeViewCommandArgs commandArgs)
    {
        _ = Task.Run(async () => await _focusSearchInputElementFunc.Invoke().ConfigureAwait(false))
            .ConfigureAwait(false);
    }
}
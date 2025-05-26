using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Models;

public class InputFileService : IInputFileService, IBackgroundTaskGroup
{
    private readonly object _stateModificationLock = new();

    private InputFileState _inputFileState = new();
	
	public event Action? InputFileStateChanged;
	
	public InputFileState GetInputFileState() => _inputFileState;

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<InputFileServiceWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

    public void StartInputFileStateForm(
        string message,
        Func<AbsolutePath, Task> onAfterSubmitFunc,
        Func<AbsolutePath, Task<bool>> selectionIsValidFunc,
        List<InputFilePattern> inputFilePatterns)
    {
        lock (_stateModificationLock)
        {
            var inState = GetInputFileState();

            _inputFileState = inState with
            {
                SelectionIsValidFunc = selectionIsValidFunc,
                OnAfterSubmitFunc = onAfterSubmitFunc,
                InputFilePatternsList = inputFilePatterns,
                SelectedInputFilePattern = inputFilePatterns.First(),
                Message = message
            };

            goto finalize;
        }

        finalize:
        InputFileStateChanged?.Invoke();
    }

    public void SetSelectedTreeViewModel(TreeViewAbsolutePath? selectedTreeViewModel)
    {
        lock (_stateModificationLock)
        {
            var inState = GetInputFileState();

            _inputFileState = inState with
            {
                SelectedTreeViewModel = selectedTreeViewModel
            };

            goto finalize;
        }

        finalize:
        InputFileStateChanged?.Invoke();
    }

    public void SetOpenedTreeViewModel(
    	TreeViewAbsolutePath treeViewModel,
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        lock (_stateModificationLock)
        {
            var inState = GetInputFileState();

            if (treeViewModel.Item.IsDirectory)
            {
                _inputFileState = InputFileState.NewOpenedTreeViewModelHistory(
                    inState,
                    treeViewModel,
                    ideComponentRenderers,
                    commonComponentRenderers,
                    fileSystemProvider,
                    environmentProvider);

                goto finalize;
            }

            _inputFileState = inState;

            goto finalize;
        }

        finalize:
        InputFileStateChanged?.Invoke();
    }

    public void SetSelectedInputFilePattern(InputFilePattern inputFilePattern)
    {
        lock (_stateModificationLock)
        {
            var inState = GetInputFileState();

            _inputFileState = inState with
            {
                SelectedInputFilePattern = inputFilePattern
            };

            goto finalize;
        }

        finalize:
        InputFileStateChanged?.Invoke();
    }

    public void MoveBackwardsInHistory()
    {
        lock (_stateModificationLock)
        {
            var inState = GetInputFileState();

            if (inState.CanMoveBackwardsInHistory)
            {
                _inputFileState = inState with { IndexInHistory = inState.IndexInHistory - 1 };

                goto finalize;
            }

            _inputFileState = inState;

            goto finalize;
        }

        finalize:
        InputFileStateChanged?.Invoke();
    }

    public void MoveForwardsInHistory()
    {
        lock (_stateModificationLock)
        {
            var inState = GetInputFileState();

            if (inState.CanMoveForwardsInHistory)
            {
                _inputFileState = inState with { IndexInHistory = inState.IndexInHistory + 1 };

                goto finalize;
            }

            _inputFileState = inState;

            goto finalize;
        }

        finalize:
        InputFileStateChanged?.Invoke();
    }

    public void OpenParentDirectory(
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        BackgroundTaskService backgroundTaskService,
        TreeViewAbsolutePath? parentDirectoryTreeViewModel)
    {
        lock (_stateModificationLock)
        {
            var inState = GetInputFileState();

            var currentSelection = inState.OpenedTreeViewModelHistoryList[inState.IndexInHistory];

            // If has a ParentDirectory select it
            if (currentSelection.Item.ParentDirectory is not null)
            {
                var parentDirectory = currentSelection.Item.ParentDirectory;

                var parentDirectoryAbsolutePath = environmentProvider.AbsolutePathFactory(
                    parentDirectory,
                    true);

                parentDirectoryTreeViewModel = new TreeViewAbsolutePath(
                    parentDirectoryAbsolutePath,
                    ideComponentRenderers,
                    commonComponentRenderers,
                    fileSystemProvider,
                    environmentProvider,
                    false,
                    true);
            }

            if (parentDirectoryTreeViewModel is not null)
            {
                _inputFileState = InputFileState.NewOpenedTreeViewModelHistory(
                    inState,
                    parentDirectoryTreeViewModel,
                    ideComponentRenderers,
                    commonComponentRenderers,
                    fileSystemProvider,
                    environmentProvider);

                goto finalize;
            }

            _inputFileState = inState;

            goto finalize;
        }

        finalize:
        InputFileStateChanged?.Invoke();
    }

    public void RefreshCurrentSelection(
    	BackgroundTaskService backgroundTaskService,
    	TreeViewAbsolutePath? currentSelection)
    {
        lock (_stateModificationLock)
        {
            var inState = GetInputFileState();

            currentSelection = inState.OpenedTreeViewModelHistoryList[inState.IndexInHistory];

            _inputFileState = inState;

            goto finalize;
        }

        finalize:
        InputFileStateChanged?.Invoke();
    }

    public void SetSearchQuery(string searchQuery)
    {
        lock (_stateModificationLock)
        {
            var inState = GetInputFileState();

            var openedTreeViewModel = inState.OpenedTreeViewModelHistoryList[inState.IndexInHistory];

            foreach (var treeViewModel in openedTreeViewModel.ChildList)
            {
                var treeViewAbsolutePath = (TreeViewAbsolutePath)treeViewModel;

                treeViewModel.IsHidden = !treeViewAbsolutePath.Item.NameWithExtension.Contains(
                    searchQuery,
                    StringComparison.InvariantCultureIgnoreCase);
            }

            _inputFileState = inState with { SearchQuery = searchQuery };

            goto finalize;
        }

        finalize:
        InputFileStateChanged?.Invoke();
    }

    private readonly
        Queue<(IIdeComponentRenderers ideComponentRenderers, ICommonComponentRenderers commonComponentRenderers, IFileSystemProvider fileSystemProvider, IEnvironmentProvider environmentProvider, BackgroundTaskService backgroundTaskService, TreeViewAbsolutePath? parentDirectoryTreeViewModel)>
        _queue_OpenParentDirectoryAction = new();

    public void Enqueue_OpenParentDirectoryAction(
    	IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        BackgroundTaskService backgroundTaskService,
        TreeViewAbsolutePath? parentDirectoryTreeViewModel)
    {
        if (parentDirectoryTreeViewModel is not null)
        {
            lock (_workLock)
            {
                _workKindQueue.Enqueue(InputFileServiceWorkKind.OpenParentDirectoryAction);

                _queue_OpenParentDirectoryAction.Enqueue((
                    ideComponentRenderers, commonComponentRenderers, fileSystemProvider, environmentProvider, backgroundTaskService, parentDirectoryTreeViewModel));

                backgroundTaskService.Continuous_EnqueueGroup(this);
            }
        }
    }
    
    public async ValueTask Do_OpenParentDirectoryAction(
    	IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        BackgroundTaskService backgroundTaskService,
        TreeViewAbsolutePath? parentDirectoryTreeViewModel)
    {
        if (parentDirectoryTreeViewModel is not null)
            await parentDirectoryTreeViewModel.LoadChildListAsync().ConfigureAwait(false);
    }

    private readonly
        Queue<(BackgroundTaskService backgroundTaskService, TreeViewAbsolutePath? currentSelection)>
        _queue_RefreshCurrentSelectionAction = new();

    public void Enqueue_RefreshCurrentSelectionAction(
        BackgroundTaskService backgroundTaskService,
    	TreeViewAbsolutePath? currentSelection)
    {
        if (currentSelection is not null)
        {
            currentSelection.ChildList.Clear();

            lock (_workLock)
            {
                _workKindQueue.Enqueue(InputFileServiceWorkKind.RefreshCurrentSelectionAction);
                _queue_RefreshCurrentSelectionAction.Enqueue((backgroundTaskService, currentSelection));
                backgroundTaskService.Continuous_EnqueueGroup(this);
            }
        }
    }
    
    public async ValueTask Do_RefreshCurrentSelectionAction(
        BackgroundTaskService backgroundTaskService,
    	TreeViewAbsolutePath? currentSelection)
    {
        if (currentSelection is not null)
            await currentSelection.LoadChildListAsync().ConfigureAwait(false);
    }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        InputFileServiceWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case InputFileServiceWorkKind.OpenParentDirectoryAction:
            {
                var args = _queue_OpenParentDirectoryAction.Dequeue();
                return Do_OpenParentDirectoryAction(
                    args.ideComponentRenderers, args.commonComponentRenderers, args.fileSystemProvider, args.environmentProvider, args.backgroundTaskService, args.parentDirectoryTreeViewModel);
            }
            case InputFileServiceWorkKind.RefreshCurrentSelectionAction:
            {
                var args = _queue_RefreshCurrentSelectionAction.Dequeue();
                return Do_RefreshCurrentSelectionAction(
                    args.backgroundTaskService, args.currentSelection);
            }
            default:
            {
                Console.WriteLine($"{nameof(InputFileService)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
            }
        }
    }
}

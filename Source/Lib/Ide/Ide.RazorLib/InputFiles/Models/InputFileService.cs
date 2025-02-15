using System.Collections.Immutable;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Models;

public class InputFileService : IInputFileService, IBackgroundTaskGroup
{
	private InputFileState _inputFileState = new();
	
	public event Action? InputFileStateChanged;
	
	public InputFileState GetInputFileState() => _inputFileState;

    public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public string Name { get; } = nameof(ConfigBackgroundTaskApi);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<InputFileServiceWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

    public void ReduceStartInputFileStateFormAction(
        string message,
        Func<AbsolutePath, Task> onAfterSubmitFunc,
        Func<AbsolutePath, Task<bool>> selectionIsValidFunc,
        ImmutableArray<InputFilePattern> inputFilePatterns)
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
        
        InputFileStateChanged?.Invoke();
        return;
    }

    public void ReduceSetSelectedTreeViewModelAction(TreeViewAbsolutePath? selectedTreeViewModel)
    {
    	var inState = GetInputFileState();
    
        _inputFileState = inState with
        {
            SelectedTreeViewModel = selectedTreeViewModel
        };
        
        InputFileStateChanged?.Invoke();
        return;
    }

    public void ReduceSetOpenedTreeViewModelAction(
    	TreeViewAbsolutePath treeViewModel,
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
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
            
            InputFileStateChanged?.Invoke();
        	return;
        }

        _inputFileState = inState;
        
        InputFileStateChanged?.Invoke();
        return;
    }

    public void ReduceSetSelectedInputFilePatternAction(InputFilePattern inputFilePattern)
    {
    	var inState = GetInputFileState();
    
        _inputFileState = inState with
        {
            SelectedInputFilePattern = inputFilePattern
        };
        
        InputFileStateChanged?.Invoke();
        return;
    }

    public void ReduceMoveBackwardsInHistoryAction()
    {
    	var inState = GetInputFileState();
    
        if (inState.CanMoveBackwardsInHistory)
        {
            _inputFileState = inState with { IndexInHistory = inState.IndexInHistory - 1 };
            
            InputFileStateChanged?.Invoke();
        	return;
        }

        _inputFileState = inState;
        
        InputFileStateChanged?.Invoke();
        return;
    }

    public void ReduceMoveForwardsInHistoryAction()
    {
    	var inState = GetInputFileState();
    
        if (inState.CanMoveForwardsInHistory)
        {
            _inputFileState = inState with { IndexInHistory = inState.IndexInHistory + 1 };
            
            InputFileStateChanged?.Invoke();
        	return;
        }

        _inputFileState = inState;
        
        InputFileStateChanged?.Invoke();
        return;
    }

    public void ReduceOpenParentDirectoryAction(
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IBackgroundTaskService backgroundTaskService,
        TreeViewAbsolutePath? parentDirectoryTreeViewModel)
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
                
            InputFileStateChanged?.Invoke();
        	return;
        }

        _inputFileState = inState;
        
        InputFileStateChanged?.Invoke();
        return;
    }

    public void ReduceRefreshCurrentSelectionAction(
    	IBackgroundTaskService backgroundTaskService,
    	TreeViewAbsolutePath? currentSelection)
    {
    	var inState = GetInputFileState();
    
        currentSelection = inState.OpenedTreeViewModelHistoryList[inState.IndexInHistory];
        
        _inputFileState = inState;
        
        InputFileStateChanged?.Invoke();
        return;
    }

    public void ReduceSetSearchQueryAction(string searchQuery)
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
        
        InputFileStateChanged?.Invoke();
        return;
    }

    private readonly
        Queue<(IIdeComponentRenderers ideComponentRenderers, ICommonComponentRenderers commonComponentRenderers, IFileSystemProvider fileSystemProvider, IEnvironmentProvider environmentProvider, IBackgroundTaskService backgroundTaskService, TreeViewAbsolutePath? parentDirectoryTreeViewModel)>
        _queue_OpenParentDirectoryAction = new();

    public void Enqueue_OpenParentDirectoryAction(
    	IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IBackgroundTaskService backgroundTaskService,
        TreeViewAbsolutePath? parentDirectoryTreeViewModel)
    {
        if (parentDirectoryTreeViewModel is not null)
        {
            lock (_workLock)
            {
                _workKindQueue.Enqueue(InputFileServiceWorkKind.OpenParentDirectoryAction);

                _queue_OpenParentDirectoryAction.Enqueue((
                    ideComponentRenderers, commonComponentRenderers, fileSystemProvider, environmentProvider, backgroundTaskService, parentDirectoryTreeViewModel));

                backgroundTaskService.EnqueueGroup(this);
            }
        }
    }
    
    public async ValueTask Do_OpenParentDirectoryAction(
    	IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IBackgroundTaskService backgroundTaskService,
        TreeViewAbsolutePath? parentDirectoryTreeViewModel)
    {
        if (parentDirectoryTreeViewModel is not null)
            await parentDirectoryTreeViewModel.LoadChildListAsync().ConfigureAwait(false);
    }

    private readonly
        Queue<(IBackgroundTaskService backgroundTaskService, TreeViewAbsolutePath? currentSelection)>
        _queue_RefreshCurrentSelectionAction = new();

    public void Enqueue_RefreshCurrentSelectionAction(
        IBackgroundTaskService backgroundTaskService,
    	TreeViewAbsolutePath? currentSelection)
    {
        if (currentSelection is not null)
        {
            currentSelection.ChildList.Clear();

            lock (_workLock)
            {
                _workKindQueue.Enqueue(InputFileServiceWorkKind.RefreshCurrentSelectionAction);
                _queue_RefreshCurrentSelectionAction.Enqueue((backgroundTaskService, currentSelection));
                backgroundTaskService.EnqueueGroup(this);
            }
        }
    }
    
    public async ValueTask Do_RefreshCurrentSelectionAction(
        IBackgroundTaskService backgroundTaskService,
    	TreeViewAbsolutePath? currentSelection)
    {
        if (currentSelection is not null)
            await currentSelection.LoadChildListAsync().ConfigureAwait(false);
    }

    public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
    {
        return null;
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
                return ValueTask.CompletedTask;
            }
        }
    }
}

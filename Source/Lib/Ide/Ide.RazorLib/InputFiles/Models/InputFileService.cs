using System.Collections.Immutable;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Models;

public class InputFileService : IInputFileService
{
	private InputFileState _inputFileState = new();
	
	public event Action? InputFileStateChanged;
	
	public InputFileState GetInputFileState() => _inputFileState;

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
        LuthetusCommonApi commonApi,
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
    
    public Task HandleOpenParentDirectoryAction(
    	IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IBackgroundTaskService backgroundTaskService,
        TreeViewAbsolutePath? parentDirectoryTreeViewModel)
    {
        if (parentDirectoryTreeViewModel is not null)
        {
            backgroundTaskService.Enqueue(
                Key<IBackgroundTask>.NewKey(),
                BackgroundTaskFacts.ContinuousQueueKey,
                "Open Parent Directory",
                async () =>
                {
                    await parentDirectoryTreeViewModel.LoadChildListAsync().ConfigureAwait(false);
                });
        }

        return Task.CompletedTask;
    }
    
    public Task HandleRefreshCurrentSelectionAction(
        IBackgroundTaskService backgroundTaskService,
    	TreeViewAbsolutePath? currentSelection)
    {
        if (currentSelection is not null)
        {
            currentSelection.ChildList.Clear();

            backgroundTaskService.Enqueue(
                Key<IBackgroundTask>.NewKey(),
                BackgroundTaskFacts.ContinuousQueueKey,
                "Refresh Current Selection",
                async () =>
                {
                    await currentSelection.LoadChildListAsync().ConfigureAwait(false);
                    // TODO: This still needs to re-render.
                });
        }

        return Task.CompletedTask;
    }
}

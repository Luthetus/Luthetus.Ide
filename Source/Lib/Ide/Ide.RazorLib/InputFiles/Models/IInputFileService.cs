using System.Collections.Immutable;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Models;

public interface IInputFileService
{
	public event Action? InputFileStateChanged;
	
	public InputFileState GetInputFileState();

    public void ReduceStartInputFileStateFormAction(
        string message,
        Func<AbsolutePath, Task> onAfterSubmitFunc,
        Func<AbsolutePath, Task<bool>> selectionIsValidFunc,
        ImmutableArray<InputFilePattern> inputFilePatterns);

    public void ReduceSetSelectedTreeViewModelAction(TreeViewAbsolutePath? SelectedTreeViewModel);

    public void ReduceSetOpenedTreeViewModelAction(
    	TreeViewAbsolutePath treeViewModel,
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider);

    public void ReduceSetSelectedInputFilePatternAction(InputFilePattern inputFilePattern);

    public void ReduceMoveBackwardsInHistoryAction();

    public void ReduceMoveForwardsInHistoryAction();

    public void ReduceOpenParentDirectoryAction(
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IBackgroundTaskService backgroundTaskService,
        TreeViewAbsolutePath? parentDirectoryTreeViewModel);

    public void ReduceRefreshCurrentSelectionAction(
    	IBackgroundTaskService backgroundTaskService,
    	TreeViewAbsolutePath? currentSelection);

    public void ReduceSetSearchQueryAction(string searchQuery);
    
    public Task HandleOpenParentDirectoryAction(
    	IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IBackgroundTaskService backgroundTaskService,
        TreeViewAbsolutePath? parentDirectoryTreeViewModel);
    
    public Task HandleRefreshCurrentSelectionAction(
        IBackgroundTaskService backgroundTaskService,
    	TreeViewAbsolutePath? currentSelection);
}

using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Models;

public interface IInputFileService
{
	public event Action? InputFileStateChanged;
	
	public InputFileState GetInputFileState();

    public void StartInputFileStateForm(
        string message,
        Func<AbsolutePath, Task> onAfterSubmitFunc,
        Func<AbsolutePath, Task<bool>> selectionIsValidFunc,
        List<InputFilePattern> inputFilePatterns);

    public void SetSelectedTreeViewModel(TreeViewAbsolutePath? SelectedTreeViewModel);

    public void SetOpenedTreeViewModel(
    	TreeViewAbsolutePath treeViewModel,
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider);

    public void SetSelectedInputFilePattern(InputFilePattern inputFilePattern);

    public void MoveBackwardsInHistory();

    public void MoveForwardsInHistory();

    public void OpenParentDirectory(
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IBackgroundTaskService backgroundTaskService,
        TreeViewAbsolutePath? parentDirectoryTreeViewModel);

    public void RefreshCurrentSelection(
    	IBackgroundTaskService backgroundTaskService,
    	TreeViewAbsolutePath? currentSelection);

    public void SetSearchQuery(string searchQuery);
    
    public void Enqueue_OpenParentDirectoryAction(
    	IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IBackgroundTaskService backgroundTaskService,
        TreeViewAbsolutePath? parentDirectoryTreeViewModel);
    
    public void Enqueue_RefreshCurrentSelectionAction(
        IBackgroundTaskService backgroundTaskService,
    	TreeViewAbsolutePath? currentSelection);
}

namespace Luthetus.Ide.RazorLib.FolderExplorers.Models;

public class FolderExplorerService : IFolderExplorerService
{
    private readonly object _stateModificationLock = new();

    private FolderExplorerState _folderExplorerState = new();
	
	public event Action? FolderExplorerStateChanged;
	
	public FolderExplorerState GetFolderExplorerState() => _folderExplorerState;

    public void With(Func<FolderExplorerState, FolderExplorerState> withFunc)
    {
        lock (_stateModificationLock)
        {
    	    var inState = GetFolderExplorerState();
            _folderExplorerState = withFunc.Invoke(inState);
            goto finalize;
        }

        finalize:
        FolderExplorerStateChanged?.Invoke();
    }
}

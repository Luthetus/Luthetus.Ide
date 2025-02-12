namespace Luthetus.Ide.RazorLib.FolderExplorers.Models;

public class FolderExplorerService : IFolderExplorerService
{
	private FolderExplorerState _folderExplorerState = new();
	
	public event Action? FolderExplorerStateChanged;
	
	public FolderExplorerState GetFolderExplorerState() => _folderExplorerState;

    public void ReduceWithAction(Func<FolderExplorerState, FolderExplorerState> withFunc)
    {
    	var inState = GetFolderExplorerState();
    
        _folderExplorerState = withFunc.Invoke(inState);
        
        FolderExplorerStateChanged?.Invoke();
        return;
    }
}

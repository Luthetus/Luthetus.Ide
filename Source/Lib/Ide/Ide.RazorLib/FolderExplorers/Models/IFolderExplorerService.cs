namespace Luthetus.Ide.RazorLib.FolderExplorers.Models;

public interface IFolderExplorerService
{
	public event Action? FolderExplorerStateChanged;
	
	public FolderExplorerState GetFolderExplorerState();

    public void ReduceWithAction(Func<FolderExplorerState, FolderExplorerState> withFunc);
}

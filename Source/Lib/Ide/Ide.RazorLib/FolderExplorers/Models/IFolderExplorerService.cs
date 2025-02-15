namespace Luthetus.Ide.RazorLib.FolderExplorers.Models;

public interface IFolderExplorerService
{
	public event Action? FolderExplorerStateChanged;
	
	public FolderExplorerState GetFolderExplorerState();

    public void With(Func<FolderExplorerState, FolderExplorerState> withFunc);
}

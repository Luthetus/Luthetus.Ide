namespace Luthetus.Ide.RazorLib.FolderExplorers.States;

public partial record FolderExplorerState
{
    public record WithAction(Func<FolderExplorerState, FolderExplorerState> WithFunc);
}
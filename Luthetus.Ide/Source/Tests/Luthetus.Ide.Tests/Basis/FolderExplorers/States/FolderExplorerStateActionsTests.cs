namespace Luthetus.Ide.Tests.Basis.FolderExplorers.States;

public class FolderExplorerStateActionsTests
{
    public record WithAction(Func<FolderExplorerState, FolderExplorerState> WithFunc);
}
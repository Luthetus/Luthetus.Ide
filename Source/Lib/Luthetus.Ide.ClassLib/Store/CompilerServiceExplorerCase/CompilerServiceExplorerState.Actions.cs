namespace Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase;

public partial record CompilerServiceExplorerState
{
    public record SetCompilerServiceExplorerAction();

    private record WithAction(Func<CompilerServiceExplorerState, CompilerServiceExplorerState> WithFunc);
    private record SetCompilerServiceExplorerTreeViewAction;
}
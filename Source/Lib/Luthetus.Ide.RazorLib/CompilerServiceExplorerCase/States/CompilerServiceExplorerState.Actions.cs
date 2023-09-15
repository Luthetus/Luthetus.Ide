namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;

public partial class CompilerServiceExplorerState
{
    public record SetCompilerServiceExplorerTreeViewTask(CompilerServiceExplorerSync Sync);
    public record NewAction(Func<CompilerServiceExplorerState, CompilerServiceExplorerState> NewFunc);
}
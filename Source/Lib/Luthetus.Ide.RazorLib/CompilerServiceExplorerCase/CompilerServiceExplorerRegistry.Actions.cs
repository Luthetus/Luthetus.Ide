namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase;

public partial class CompilerServiceExplorerRegistry
{
    public record SetCompilerServiceExplorerAction();
    public record SetViewKindAction(CompilerServiceExplorerViewKind viewKind);

    private record NewAction(Func<CompilerServiceExplorerRegistry, CompilerServiceExplorerRegistry> NewFunc);
    private record SetCompilerServiceExplorerTreeViewAction;
}
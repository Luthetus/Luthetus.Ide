using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase;

namespace Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase;

public partial class CompilerServiceExplorerRegistry
{
    public record SetCompilerServiceExplorerAction();
    public record SetViewKindAction(CompilerServiceExplorerViewKind viewKind);

    private record NewAction(Func<CompilerServiceExplorerRegistry, CompilerServiceExplorerRegistry> NewFunc);
    private record SetCompilerServiceExplorerTreeViewAction;
}
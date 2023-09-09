using Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase.InnerDetails;

namespace Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase;

public partial class CompilerServiceExplorerState
{
    public record SetCompilerServiceExplorerAction();
    public record SetViewKindAction(CompilerServiceExplorerViewKind viewKind);

    private record NewAction(Func<CompilerServiceExplorerState, CompilerServiceExplorerState> NewFunc);
    private record SetCompilerServiceExplorerTreeViewAction;
}
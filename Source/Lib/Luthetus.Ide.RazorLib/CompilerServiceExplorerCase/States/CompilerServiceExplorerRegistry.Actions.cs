using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Models;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;

public partial class CompilerServiceExplorerRegistry
{
    public record SetCompilerServiceExplorerAction();
    public record SetViewKindAction(CompilerServiceExplorerViewKind viewKind);

    private record NewAction(Func<CompilerServiceExplorerRegistry, CompilerServiceExplorerRegistry> NewFunc);
    private record SetCompilerServiceExplorerTreeViewAction;
}
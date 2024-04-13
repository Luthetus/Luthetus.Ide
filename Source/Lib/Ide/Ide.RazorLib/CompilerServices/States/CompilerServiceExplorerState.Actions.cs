namespace Luthetus.Ide.RazorLib.CompilerServices.States;

public partial class CompilerServiceExplorerState
{
    public record NewAction(Func<CompilerServiceExplorerState, CompilerServiceExplorerState> NewFunc);
}
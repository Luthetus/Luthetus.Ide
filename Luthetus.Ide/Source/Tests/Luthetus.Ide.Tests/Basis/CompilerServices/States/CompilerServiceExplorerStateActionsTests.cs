namespace Luthetus.Ide.Tests.Basis.CompilerServices.States;

public class CompilerServiceExplorerStateActionsTests
{
    public record NewAction(Func<CompilerServiceExplorerState, CompilerServiceExplorerState> NewFunc);
}
namespace Luthetus.Extensions.DotNet.CompilerServices.States;

public partial class CompilerServiceExplorerState
{
    public record NewAction(Func<CompilerServiceExplorerState, CompilerServiceExplorerState> NewFunc);
}
namespace Luthetus.Ide.Tests.Basis.CodeSearches.States;

public class CodeSearchStateActionsTests
{
    public record WithAction(Func<CodeSearchState, CodeSearchState> WithFunc);
    public record AddResultAction(string Result);
    public record ClearResultListAction;
    public record SearchEffect(CancellationToken CancellationToken = default);
}

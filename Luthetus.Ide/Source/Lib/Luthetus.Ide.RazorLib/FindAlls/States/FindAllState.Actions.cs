namespace Luthetus.Ide.RazorLib.FindAlls.States;

public partial record FindAllState
{
    public record WithAction(Func<FindAllState, FindAllState> WithFunc);
    public record AddResultAction(string Result);
    public record ClearResultListAction;
    public record SearchEffect(CancellationToken CancellationToken = default);
}

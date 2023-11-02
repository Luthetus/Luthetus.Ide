namespace Luthetus.Common.RazorLib.Options.States;

public partial record AppOptionsStateTests
{
    public record WithAction(Func<AppOptionsState, AppOptionsState> WithFunc);
}
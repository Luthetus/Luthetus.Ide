namespace Luthetus.Common.RazorLib.Options.States;

public partial record AppOptionsState
{
    public record WithAction(Func<AppOptionsState, AppOptionsState> WithFunc);
}
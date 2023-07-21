namespace Luthetus.Ide.RazorLib.Terminal;

public partial class TerminalDisplay : FluxorComponent
{
    [Inject]
    private IState<WellKnownTerminalSessionsState> WellKnownTerminalSessionsStateWrap { get; set; } = null!;
}
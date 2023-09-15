using Fluxor;
using Luthetus.Ide.RazorLib.TerminalCase.Models;

namespace Luthetus.Ide.RazorLib.TerminalCase.States;

[FeatureState]
public partial record WellKnownTerminalSessionsState(TerminalSessionKey ActiveTerminalSessionKey)
{
    public WellKnownTerminalSessionsState() : this(TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY)
    {
    }
}

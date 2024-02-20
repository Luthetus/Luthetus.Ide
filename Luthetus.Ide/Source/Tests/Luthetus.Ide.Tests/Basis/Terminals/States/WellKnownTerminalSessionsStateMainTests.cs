using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.Tests.Basis.Terminals.States;

[FeatureState]
public partial record WellKnownTerminalSessionsStateMainTests(Key<TerminalSession> ActiveTerminalSessionKey)
{
    public WellKnownTerminalSessionsState() : this(TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY)
    {
    }
}

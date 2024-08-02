using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Terminals.States;

[FeatureState]
public partial record WellKnownTerminalState(Key<ITerminal> ActiveTerminalKey)
{
    public WellKnownTerminalState() : this(TerminalFacts.GENERAL_TERMINAL_KEY)
    {
    }
}

using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record WellKnownTerminalState
{
    public record SetActiveWellKnownTerminalKey(Key<Terminal> TerminalKey);
}
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.TerminalCase.States;

public partial record WellKnownTerminalSessionsState
{
    public record SetActiveWellKnownTerminalSessionKey(Key<TerminalSession> TerminalSessionKey);
}
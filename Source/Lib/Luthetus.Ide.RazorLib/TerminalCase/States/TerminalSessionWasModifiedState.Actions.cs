using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.StateCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.Models;

namespace Luthetus.Ide.RazorLib.TerminalCase.States;

public partial record TerminalSessionWasModifiedState
{
    public record SetTerminalSessionStateKeyAction(
        Key<TerminalSession> TerminalSessionKey,
        Key<StateRecord> StateKey);
}
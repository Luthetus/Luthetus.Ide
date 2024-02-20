using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.States.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.Tests.Basis.Terminals.States;

public partial record TerminalSessionWasModifiedStateActionsTests
{
    public record SetTerminalSessionStateKeyAction(
        Key<TerminalSession> TerminalSessionKey,
        Key<StateRecord> StateKey);
}
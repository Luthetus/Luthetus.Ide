using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.Tests.Basis.Terminals.States;

public partial record WellKnownTerminalSessionsStateActionsTests
{
    public record SetActiveWellKnownTerminalSessionKey(Key<TerminalSession> TerminalSessionKey);
}
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.Tests.Basis.Terminals.States;

public partial record TerminalSessionStateActionsTests
{
    public record RegisterTerminalSessionAction(TerminalSession TerminalSession);
    public record UpdateTerminalSessionStateKeyAction(TerminalSession TerminalSession);
    public record DisposeTerminalSessionAction(Key<TerminalSession> TerminalSessionKey);
}
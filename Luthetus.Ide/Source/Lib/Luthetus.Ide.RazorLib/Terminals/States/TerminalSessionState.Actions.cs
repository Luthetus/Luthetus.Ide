using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record TerminalSessionState
{
    public record RegisterTerminalSessionAction(TerminalSession TerminalSession);
    public record NotifyStateChangedAction(Key<TerminalSession> TerminalSessionKey);
    public record DisposeTerminalSessionAction(Key<TerminalSession> TerminalSessionKey);
}
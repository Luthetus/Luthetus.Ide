using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record TerminalState
{
    public record RegisterAction(Terminal Terminal);
    public record NotifyStateChangedAction(Key<Terminal> TerminalKey);
    public record DisposeAction(Key<Terminal> TerminalKey);
}
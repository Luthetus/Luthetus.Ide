using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record TerminalState
{
    public record RegisterAction(ITerminal Terminal);
    public record NotifyStateChangedAction(Key<ITerminal> TerminalKey);
    public record DisposeAction(Key<ITerminal> TerminalKey);
}
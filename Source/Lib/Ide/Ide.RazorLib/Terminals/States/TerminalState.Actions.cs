using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record TerminalState
{
    public record RegisterAction(Terminal Terminal);
    public record NotifyStateChangedAction(Key<Terminal> TerminalKey);
    public record DisposeAction(Key<Terminal> TerminalKey);
    
    public record NEW_TERMINAL_CODE_RegisterAction(ITerminal NEW_TERMINAL);
    public record EXECUTION_TERMINAL_CODE_RegisterAction(ITerminal NEW_TERMINAL);
}
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public interface ITerminalService
{
	public event Action? TerminalStateChanged;

	public TerminalState GetTerminalState();

    public void ReduceRegisterAction(ITerminal terminal);
    public void ReduceStateHasChangedAction();
    public void ReduceDisposeAction(Key<ITerminal> terminalKey);
}

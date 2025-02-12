using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalService : ITerminalService
{
	private TerminalState _terminalState = new();

	public event Action? TerminalStateChanged;

	public TerminalState GetTerminalState() => _terminalState;

    public void ReduceRegisterAction(ITerminal terminal)
    {
    	var inState = GetTerminalState();
    
        if (inState.TerminalMap.ContainsKey(terminal.Key))
        {
            TerminalStateChanged?.Invoke();
            return;
        }

        var nextMap = inState.TerminalMap.Add(
            terminal.Key,
            terminal);

        _terminalState = inState with { TerminalMap = nextMap };
        
        TerminalStateChanged?.Invoke();
        return;
    }

    public void ReduceStateHasChangedAction()
    {
    	TerminalStateChanged?.Invoke();
        return;
    }

    public void ReduceDisposeAction(Key<ITerminal> terminalKey)
    {
    	var inState = GetTerminalState();
    
        var nextMap = inState.TerminalMap.Remove(terminalKey);
        _terminalState = inState with { TerminalMap = nextMap };
        
        TerminalStateChanged?.Invoke();
        return;
    }
}

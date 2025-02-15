using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalService : ITerminalService
{
    private readonly object _stateModificationLock = new();

    private TerminalState _terminalState = new();

	public event Action? TerminalStateChanged;

	public TerminalState GetTerminalState() => _terminalState;

    public void Register(ITerminal terminal)
    {
        lock (_stateModificationLock)
        {
            var inState = GetTerminalState();

            if (inState.TerminalMap.ContainsKey(terminal.Key))
                goto finalize;

            var nextMap = new Dictionary<Key<ITerminal>, ITerminal>(inState.TerminalMap);
            nextMap.Add(terminal.Key, terminal);

            _terminalState = inState with { TerminalMap = nextMap };

            goto finalize;
        }

        finalize:
        TerminalStateChanged?.Invoke();
    }

    public void StateHasChanged()
    {
    	TerminalStateChanged?.Invoke();
    }

    public void Dispose(Key<ITerminal> terminalKey)
    {
        lock (_stateModificationLock)
        {
            var inState = GetTerminalState();
            
            var nextMap = new Dictionary<Key<ITerminal>, ITerminal>(inState.TerminalMap);
            nextMap.Remove(terminalKey);

            _terminalState = inState with { TerminalMap = nextMap };

            goto finalize;
        }

        finalize:
        TerminalStateChanged?.Invoke();
    }
}

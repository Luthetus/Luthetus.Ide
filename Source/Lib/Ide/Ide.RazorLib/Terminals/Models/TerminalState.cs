using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public record struct TerminalState(Dictionary<Key<ITerminal>, ITerminal> TerminalMap)
{
    private static readonly Dictionary<Key<ITerminal>, ITerminal> _empty = new();

    public TerminalState() : this(_empty)
    {
    }

    public Dictionary<Key<ITerminal>, ITerminal> GetEmpty()
    {
        if (_empty.Count != 0)
            Console.WriteLine($"{nameof(TerminalState)}.{nameof(GetEmpty)} did not have a count of 0.");

        return _empty;
    }
}
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public static class TerminalFacts
{
    public static readonly Key<ITerminal> EXECUTION_KEY = Key<ITerminal>.NewKey();
    public static readonly Key<ITerminal> GENERAL_KEY = Key<ITerminal>.NewKey();

    public static readonly List<Key<ITerminal>> WELL_KNOWN_KEYS = new()
    {
        EXECUTION_KEY,
        GENERAL_KEY,
    };
}
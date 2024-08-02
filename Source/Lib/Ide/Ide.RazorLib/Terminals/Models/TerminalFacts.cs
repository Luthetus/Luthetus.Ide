using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public static class TerminalFacts
{
    public static readonly Key<ITerminal> EXECUTION_TERMINAL_KEY = Key<ITerminal>.NewKey();
    public static readonly Key<ITerminal> GENERAL_TERMINAL_KEY = Key<ITerminal>.NewKey();

    public static readonly ImmutableArray<Key<ITerminal>> WELL_KNOWN_TERMINAL_KEYS = new[]
    {
        EXECUTION_TERMINAL_KEY,
        GENERAL_TERMINAL_KEY,
    }.ToImmutableArray();
}
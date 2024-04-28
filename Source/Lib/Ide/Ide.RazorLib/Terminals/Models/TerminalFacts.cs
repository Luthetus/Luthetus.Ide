using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public static class TerminalFacts
{
    public static readonly Key<Terminal> EXECUTION_TERMINAL_KEY = Key<Terminal>.NewKey();
    public static readonly Key<Terminal> GENERAL_TERMINAL_KEY = Key<Terminal>.NewKey();

    public static readonly ImmutableArray<Key<Terminal>> WELL_KNOWN_TERMINAL_KEYS = new[]
    {
        EXECUTION_TERMINAL_KEY,
        GENERAL_TERMINAL_KEY,
    }.ToImmutableArray();
}
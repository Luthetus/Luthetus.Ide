using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public static class TerminalSessionFacts
{
    public static readonly Key<TerminalSession> EXECUTION_TERMINAL_SESSION_KEY = Key<TerminalSession>.NewKey();
    public static readonly Key<TerminalSession> GENERAL_TERMINAL_SESSION_KEY = Key<TerminalSession>.NewKey();

    public static readonly ImmutableArray<Key<TerminalSession>> WELL_KNOWN_TERMINAL_SESSION_KEYS = new[]
    {
        EXECUTION_TERMINAL_SESSION_KEY,
        GENERAL_TERMINAL_SESSION_KEY
    }.ToImmutableArray();
}
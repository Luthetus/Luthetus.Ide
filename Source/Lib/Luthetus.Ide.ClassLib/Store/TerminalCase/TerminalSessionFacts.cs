using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Store.TerminalCase;

public static class TerminalSessionFacts
{
    public static readonly TerminalSessionKey EXECUTION_TERMINAL_SESSION_KEY =
        TerminalSessionKey.NewTerminalSessionKey("Execution");

    public static readonly TerminalSessionKey GENERAL_TERMINAL_SESSION_KEY =
        TerminalSessionKey.NewTerminalSessionKey("General");

    public static readonly ImmutableArray<TerminalSessionKey> WELL_KNOWN_TERMINAL_SESSION_KEYS = new[]
    {
        EXECUTION_TERMINAL_SESSION_KEY,
        GENERAL_TERMINAL_SESSION_KEY
    }.ToImmutableArray();
}
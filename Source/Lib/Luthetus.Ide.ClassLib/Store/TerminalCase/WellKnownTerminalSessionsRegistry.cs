using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.TerminalCase;

[FeatureState]
public record WellKnownTerminalSessionsRegistry(TerminalSessionKey ActiveTerminalSessionKey)
{
    public WellKnownTerminalSessionsRegistry() : this(TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY)
    {
    }

    public record SetActiveWellKnownTerminalSessionKey(TerminalSessionKey TerminalCommandKey);
}
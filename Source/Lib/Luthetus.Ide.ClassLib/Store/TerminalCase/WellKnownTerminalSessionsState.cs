namespace Luthetus.Ide.ClassLib.Store.TerminalCase;

[FeatureState]
public record WellKnownTerminalSessionsState(TerminalSessionKey ActiveTerminalSessionKey)
{
    public WellKnownTerminalSessionsState() : this(TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY)
    {
    }

    public record SetActiveWellKnownTerminalSessionKey(TerminalSessionKey TerminalCommandKey);
}
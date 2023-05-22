using System.Collections.Immutable;
using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.TerminalCase;

[FeatureState]
public record TerminalSessionsState(ImmutableDictionary<TerminalSessionKey, TerminalSession> TerminalSessionMap)
{
    public TerminalSessionsState()
        : this(ImmutableDictionary<TerminalSessionKey, TerminalSession>.Empty)
    {
    }
}
using Fluxor;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Store.TerminalCase;

[FeatureState]
public record TerminalSessionRegistry(ImmutableDictionary<TerminalSessionKey, TerminalSession> TerminalSessionMap)
{
    public TerminalSessionRegistry()
        : this(ImmutableDictionary<TerminalSessionKey, TerminalSession>.Empty)
    {
    }
}
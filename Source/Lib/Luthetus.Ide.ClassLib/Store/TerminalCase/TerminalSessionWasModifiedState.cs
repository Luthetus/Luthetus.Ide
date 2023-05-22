using System.Collections.Immutable;
using Fluxor;
using Luthetus.Ide.ClassLib.State;

namespace Luthetus.Ide.ClassLib.Store.TerminalCase;

[FeatureState]
public record TerminalSessionWasModifiedState(ImmutableDictionary<TerminalSessionKey, StateKey> TerminalSessionWasModifiedMap)
{
    public TerminalSessionWasModifiedState()
        : this(ImmutableDictionary<TerminalSessionKey, StateKey>.Empty)
    {
    }
}
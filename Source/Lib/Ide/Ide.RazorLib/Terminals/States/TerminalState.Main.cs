using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Terminals.States;

[FeatureState]
public partial record TerminalState(
	ImmutableDictionary<Key<ITerminal>, ITerminal> TerminalMap)
{
    public TerminalState()
        : this(ImmutableDictionary<Key<ITerminal>, ITerminal>.Empty)
    {
    }
}
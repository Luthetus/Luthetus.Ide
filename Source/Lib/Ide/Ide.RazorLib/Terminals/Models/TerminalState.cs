using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public record struct TerminalState(
	ImmutableDictionary<Key<ITerminal>, ITerminal> TerminalMap)
{
    public TerminalState()
        : this(ImmutableDictionary<Key<ITerminal>, ITerminal>.Empty)
    {
    }
}
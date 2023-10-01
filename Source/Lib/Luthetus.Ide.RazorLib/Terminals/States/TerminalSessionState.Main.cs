using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.TerminalCase.States;

[FeatureState]
public partial record TerminalSessionState(ImmutableDictionary<Key<TerminalSession>, TerminalSession> TerminalSessionMap)
{
    public TerminalSessionState()
        : this(ImmutableDictionary<Key<TerminalSession>, TerminalSession>.Empty)
    {
    }
}
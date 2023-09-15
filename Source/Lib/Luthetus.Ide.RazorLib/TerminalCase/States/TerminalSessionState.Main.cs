using Fluxor;
using Luthetus.Ide.RazorLib.TerminalCase.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.TerminalCase.States;

[FeatureState]
public partial record TerminalSessionState(ImmutableDictionary<TerminalSessionKey, TerminalSession> TerminalSessionMap)
{
    public TerminalSessionState()
        : this(ImmutableDictionary<TerminalSessionKey, TerminalSession>.Empty)
    {
    }
}
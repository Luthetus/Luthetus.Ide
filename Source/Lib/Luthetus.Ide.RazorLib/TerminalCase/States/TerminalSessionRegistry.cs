using Fluxor;
using Luthetus.Ide.RazorLib.TerminalCase.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.TerminalCase.States;

[FeatureState]
public record TerminalSessionRegistry(ImmutableDictionary<TerminalSessionKey, TerminalSession> TerminalSessionMap)
{
    public TerminalSessionRegistry()
        : this(ImmutableDictionary<TerminalSessionKey, TerminalSession>.Empty)
    {
    }
}
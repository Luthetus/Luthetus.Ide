using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.States.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.Tests.Basis.Terminals.States;

public partial record TerminalSessionWasModifiedStateMainTests(ImmutableDictionary<Key<TerminalSession>, Key<StateRecord>> TerminalSessionWasModifiedMap, string EmptyTextHack)
{
    public TerminalSessionWasModifiedState()
        : this(ImmutableDictionary<Key<TerminalSession>, Key<StateRecord>>.Empty, string.Empty)
    {
    }
}
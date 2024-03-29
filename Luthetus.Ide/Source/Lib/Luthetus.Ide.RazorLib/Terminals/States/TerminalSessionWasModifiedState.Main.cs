﻿using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.States.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Terminals.States;

/// <param name="EmptyTextHack">(2023-06-09) I added this property because I did a refactor to remove unused properties and then an injection of this state got removed. Fluxor however was using the injection due to fluxor component inheritance. So the code broke. I'm referencing this empty string so Visual Studio sees me using the property.</param>
[FeatureState]
public partial record TerminalSessionWasModifiedState(ImmutableDictionary<Key<TerminalSession>, Key<StateRecord>> TerminalSessionWasModifiedMap, string EmptyTextHack)
{
    public TerminalSessionWasModifiedState()
        : this(ImmutableDictionary<Key<TerminalSession>, Key<StateRecord>>.Empty, string.Empty)
    {
    }
}
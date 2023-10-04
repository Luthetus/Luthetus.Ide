using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.States;

[FeatureState]
public partial record DropdownState(ImmutableList<Key<DropdownRecord>> ActiveKeyBag)
{
    private DropdownState() : this(ImmutableList<Key<DropdownRecord>>.Empty)
    {

    }
}
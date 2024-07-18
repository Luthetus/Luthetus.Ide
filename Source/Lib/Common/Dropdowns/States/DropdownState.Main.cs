using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.States;

[FeatureState]
public partial record DropdownState(ImmutableList<DropdownRecord> DropdownList)
{
    public DropdownState() : this(ImmutableList<DropdownRecord>.Empty)
    {
		
    }
}
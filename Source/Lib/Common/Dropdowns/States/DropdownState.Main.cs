using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.States;

[FeatureState]
public partial record DropdownState(
	ImmutableList<Key<DropdownRecord>> ActiveKeyList,
	ImmutableList<DropdownRecord> DropdownList)
{
    public DropdownState()
		: this(ImmutableList<Key<DropdownRecord>>.Empty, ImmutableList<DropdownRecord>.Empty)
    {
		
    }
}
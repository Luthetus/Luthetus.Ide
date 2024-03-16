using Fluxor;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

namespace Luthetus.Common.RazorLib.PolymorphicViewModels.States;

[FeatureState]
public partial record PolymorphicViewModelState(
	ImmutableDictionary<object, IPolymorphicViewModel> Map)
{
	public PolymorphicViewModelState() : this(ImmutableDictionary<object, IPolymorphicViewModel>.Empty)
	{
	}
}

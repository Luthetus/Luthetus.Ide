using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

namespace Luthetus.Common.RazorLib.PolymorphicUis.States;

[FeatureState]
public partial record PolymorphicViewModelState
{
	public ImmutableDictionary<FullTypeName, IPolymorphicUiRecord> Map { get; init }
}



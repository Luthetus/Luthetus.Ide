using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Utility.Models;

namespace Luthetus.Common.RazorLib.PolymorphicViewModels.States;

[FeatureState]
public partial record PolymorphicViewModelState
{
	public ImmutableDictionary<FullTypeName, IPolymorphicUiRecord> Map { get; init; }
}



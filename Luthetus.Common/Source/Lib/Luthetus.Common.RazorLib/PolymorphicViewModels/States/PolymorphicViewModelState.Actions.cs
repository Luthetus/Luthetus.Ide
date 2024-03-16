using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

namespace Luthetus.Common.RazorLib.PolymorphicViewModels.States;

public partial record PolymorphicViewModelState
{
	public record RegisterAction(Guid Guid, IPolymorphicViewModel PolymorphicViewModel);
	public record DisposeAction(Guid Guid);
}

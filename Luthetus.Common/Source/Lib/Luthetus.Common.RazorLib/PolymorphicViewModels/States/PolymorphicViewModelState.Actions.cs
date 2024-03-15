using Luthetus.Common.RazorLib.Utility.Models;

namespace Luthetus.Common.RazorLib.PolymorphicViewModels.States;

public partial record PolymorphicViewModelState
{
	public record RegisterAction(FullTypeName FullTypeName, IPolymorphicUiRecord PolymorphicUiRecord);
	public record DisposeAction(FullTypeName FullTypeName);
}

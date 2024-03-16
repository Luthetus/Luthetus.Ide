using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

namespace Luthetus.Common.RazorLib.Drags.Models;

public class DropzoneViewModel : IDropzoneViewModel
{
	public DropzoneViewModel(IPolymorphicViewModel? polymorphicViewModel)
	{
		PolymorphicViewModel = polymorphicViewModel;
	}
	
	public IPolymorphicViewModel? PolymorphicViewModel { get; init; }
	public Key<IDropzoneViewModel> Key { get; init; }
	public MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions { get; init; }
	public ElementDimensions DropzoneElementDimensions { get; init; }
	public string? CssClassString { get; init; }
}

using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

namespace Luthetus.Common.RazorLib.Drags.Models;

public interface IDropzoneViewModel
{
	public IPolymorphicViewModel? PolymorphicViewModel { get; }
	public Key<IDropzoneViewModel> Key { get; }
	public MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions { get; }
	public ElementDimensions DropzoneElementDimensions { get; }
	public string? CssClassString { get; }	
}

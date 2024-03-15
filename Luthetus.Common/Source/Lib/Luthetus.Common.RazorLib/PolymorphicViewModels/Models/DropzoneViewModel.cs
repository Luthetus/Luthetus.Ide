using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.Common.RazorLib.PolymorphicViewModels.Models;

public interface IDropzoneViewModel
{
	public MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions { get; }
	public ElementDimensions DropzoneElementDimensions { get; }
	public string? CssClassString { get; }
}

public class DropzoneViewModel : IDropzoneViewModel
{
	public MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions { get; init; }
	public ElementDimensions DropzoneElementDimensions { get; init; }
	public string? CssClassString { get; init; }
}

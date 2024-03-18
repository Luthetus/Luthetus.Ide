using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Drags.Models;

public class DropzoneViewModel : IDropzone
{
	public DropzoneViewModel()
	{
	}
	
	public Key<IDropzone> Key { get; init; }
	public MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions { get; init; }
	public ElementDimensions DropzoneElementDimensions { get; init; }
	public string? CssClassString { get; init; }
	public Key<IDropzone> DropzoneKey { get; }
	public ElementDimensions ElementDimensions { get; }
	public string CssClass { get; }
	public string CssStyle { get; }
}

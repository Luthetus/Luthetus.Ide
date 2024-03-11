using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.Common.RazorLib.PolymorphicUis.Models;

public interface IPolymorphicDropzone
{
	public MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions { get; }
	public ElementDimensions DropzoneElementDimensions { get; }
}

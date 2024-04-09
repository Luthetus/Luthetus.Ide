using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Drags.Models;

public record DropzoneViewModel(
		Key<IDropzone> Key,
		MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions,
		ElementDimensions DropzoneElementDimensions,
        Key<IDropzone> DropzoneKey,
        ElementDimensions ElementDimensions,
        string CssClass,
        string CssStyle)
	: IDropzone;

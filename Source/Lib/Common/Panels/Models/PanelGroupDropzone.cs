using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Panels.Models;

public record PanelGroupDropzone(
        MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions,
        Key<PanelGroup> PanelGroupKey,
        ElementDimensions ElementDimensions,
        Key<IDropzone> DropzoneKey,
        string? CssClass,
        string? CssStyle)
	: IDropzone;

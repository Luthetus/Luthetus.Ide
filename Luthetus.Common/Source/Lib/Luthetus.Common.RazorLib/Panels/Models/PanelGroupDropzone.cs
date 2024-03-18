using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Panels.Models;

public class PanelGroupDropzone : IDropzone
{
	public PanelGroupDropzone(
		MeasuredHtmlElementDimensions measuredHtmlElementDimensions,
		Key<PanelGroup> panelGroupKey,
		ElementDimensions elementDimensions)
	{
		MeasuredHtmlElementDimensions = measuredHtmlElementDimensions;
		PanelGroupKey = panelGroupKey;
		ElementDimensions = elementDimensions;
	}

	public MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions { get; }
    public Key<PanelGroup> PanelGroupKey { get; }
	public Key<IDropzone> DropzoneKey { get; }
	public ElementDimensions ElementDimensions { get; }
	public string CssClass { get; init; }
	public string CssStyle { get; }
}

using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Panels.Models;

public class PanelDropzone : IPolymorphicDropzone
{
	public PanelDropzone(
		MeasuredHtmlElementDimensions measuredHtmlElementDimensions,
		ElementDimensions elementDimensions,
		Key<PanelGroup>? panelGroupKey)
	{
		MeasuredHtmlElementDimensions = measuredHtmlElementDimensions;
		DropzoneElementDimensions = elementDimensions;
		PanelGroupKey = panelGroupKey;
	}

	public MeasuredHtmlElementDimensions MeasuredHtmlElementDimensions { get; }
	public ElementDimensions DropzoneElementDimensions { get; }
	/// <summary>
	/// If the group key is not null, then upon mouse up, add the view model to the group,
	/// and set the panel key as being active.
	/// </summary>
	public Key<PanelGroup>? PanelGroupKey { get; }
}

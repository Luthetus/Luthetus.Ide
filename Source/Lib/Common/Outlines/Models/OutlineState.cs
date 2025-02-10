using Fluxor;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.Common.RazorLib.Outlines.Models;

public record struct OutlineState(
	string? ElementId,
	MeasuredHtmlElementDimensions? MeasuredHtmlElementDimensions,
	bool NeedsMeasured)
{
	public OutlineState() : this(null, null, false)
	{
	}
}

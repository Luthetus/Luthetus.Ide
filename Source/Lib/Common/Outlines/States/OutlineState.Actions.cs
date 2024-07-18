using Luthetus.Common.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.Common.RazorLib.Outlines.States;

public partial record OutlineState
{
	public record SetOutlineAction(
		string? ElementId,
		MeasuredHtmlElementDimensions? MeasuredHtmlElementDimensions,
		bool NeedsMeasured);
		
	/// <summary>
	/// The element which was measured is included in order to "handshake" that
	/// the element being outlined did not change out from under us.
	///
	/// If the element did happen to change out from under us, then this action
	/// will not do anything.
	/// </summary>
	public record SetMeasurementsAction(
		string? ElementId,
		MeasuredHtmlElementDimensions? MeasuredHtmlElementDimensions);
}

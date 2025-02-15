using Luthetus.Common.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.Common.RazorLib.Outlines.Models;

public interface IOutlineService
{
	public event Action? OutlineStateChanged;
	
	public OutlineState GetOutlineState();

	public void SetOutline(
		string? elementId,
		MeasuredHtmlElementDimensions? measuredHtmlElementDimensions,
		bool needsMeasured);
	
	/// <summary>
	/// The element which was measured is included in order to "handshake" that
	/// the element being outlined did not change out from under us.
	///
	/// If the element did happen to change out from under us, then this action
	/// will not do anything.
	/// </summary>
	public void SetMeasurements(
		string? elementId,
		MeasuredHtmlElementDimensions? measuredHtmlElementDimensions);
}

using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.Models;

/// <summary>
/// The unit of measurement is Pixels (px)
/// </summary>
/// <param name="Width">The unit of measurement is Pixels (px)</param>
/// <param name="Height">The unit of measurement is Pixels (px)</param>
/// <param name="Left">The unit of measurement is Pixels (px)</param>
/// <param name="Top">The unit of measurement is Pixels (px)</param>
public record DropdownRecord
{
	public DropdownRecord(
		Key<DropdownRecord> key,
		double leftInitial,
		double topInitial,
		Type componentType,
		Dictionary<string, object?>? componentParameterMap,
		Func<Task>? restoreFocusOnClose)
	{
		Key = key;
		Left = leftInitial;
		Top = topInitial;
		ComponentType = componentType;
		ComponentParameterMap = componentParameterMap;
		RestoreFocusOnClose = restoreFocusOnClose;
	}

	public Key<DropdownRecord> Key { get; init; }
	public double? Width { get; init; }
	public double? Height { get; init; }
	public double Left { get; init; }
	public double Top { get; init; }
	public Type ComponentType { get; init; }
	public Dictionary<string, object?>? ComponentParameterMap { get; init; }
	public Func<Task>? RestoreFocusOnClose { get; init; }
	public bool ShouldShowOutOfBoundsClickDisplay { get; init; } = true;

	/// <summary>
	/// Invoke this event via the method: <see cref="OnHtmlElementDimensionsChanged"/>.
	/// Use this event when the JavaScript code that measures if a dropdown is on-screen should be re-ran.
	/// </summary>
	public event Action? HtmlElementDimensionsChanged;

	/// <summary>
	/// This method will invoke the event: <see cref="HtmlElementDimensionsChanged"/>.
	/// Invoke this method to cause the execution of the
	/// JavaScript code that measures if a dropdown is on-screen.
	/// </summary>
	public void OnHtmlElementDimensionsChanged()
	{
		HtmlElementDimensionsChanged?.Invoke();
	}
}

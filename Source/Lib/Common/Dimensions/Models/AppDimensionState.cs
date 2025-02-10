using Fluxor;

namespace Luthetus.Common.RazorLib.Dimensions.Models;

/// <summary>
/// The measurements are in pixels (px).
/// This class is in reference to the "browser", "user agent", "desktop application which is rendering a webview", etc...
///
/// When one resizes the application, then <see cref="IDispatcher"/>.<see cref="IDispatcher.Dispatch"/>
/// the <see cref="SetAppDimensionStateAction"/>.
///
/// Any part of the application can subscribe to this state, and be notified
/// when a <see cref="SetAppDimensionStateAction"/> was reduced.
/// </summary>
///
/// <param name="Width">
/// The unit of measurement is Pixels (px).
/// This describes the Width of the application.
/// </param>
///
/// <param name="Height">
/// The unit of measurement is Pixels (px).
/// This describes the Height of the application.
/// </param>
///
/// <param name="Left">
/// The unit of measurement is Pixels (px).
/// This describes the distance the application is from the left side of the "display/monitor".
/// </param>
///
/// <param name="Top">
/// The unit of measurement is Pixels (px).
/// This describes the distance the application is from the top side of the "display/monitor".
/// </param>
public record struct AppDimensionState(int Width, int Height, int Left, int Top)
{
	public AppDimensionState() : this(0, 0, 0, 0)
	{
	}
}


using Fluxor;

namespace Luthetus.Common.RazorLib.Dimensions.States;

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
/// <param name="Width">The unit of measurement is Pixels (px)</param>
/// <param name="Height">The unit of measurement is Pixels (px)</param>
/// <param name="Left">The unit of measurement is Pixels (px)</param>
/// <param name="Top">The unit of measurement is Pixels (px)</param>
[FeatureState]
public partial record AppDimensionState(int Width, int Height, int Left, int Top)
{
	public AppDimensionState() : this(0, 0, 0, 0)
	{
	}
}


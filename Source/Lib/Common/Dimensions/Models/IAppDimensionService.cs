namespace Luthetus.Common.RazorLib.Dimensions.Models;

public interface IAppDimensionService
{
	public event Action? AppDimensionStateChanged;
	
	public AppDimensionState GetAppDimensionState();
	
	public void ReduceSetAppDimensionsAction(Func<AppDimensionState, AppDimensionState> withFunc);

	/// <summary>
	/// This action is for resizing that is done to an HTML element that is rendered.
	/// Ex: <see cref="Luthetus.Common.RazorLib.Resizes.Displays.ResizableColumn"/>
	///
	/// Since these resizes won't affect the application's dimensions as a whole,
	/// nothing needs to be used as a parameter, its just a way to notify.
	/// </summary>
	public void ReduceNotifyIntraAppResizeAction();

	/// <summary>
	/// This action is for resizing that is done to the "user agent" / "window" / "document".
	/// </summary>
	public void ReduceNotifyUserAgentResizeAction();
}

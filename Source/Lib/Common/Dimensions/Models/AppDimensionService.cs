namespace Luthetus.Common.RazorLib.Dimensions.Models;

public class AppDimensionService : IAppDimensionService
{
	private AppDimensionState _appDimensionState;
	
	public event Action? AppDimensionStateChanged;
	
	public AppDimensionState GetAppDimensionState() => _appDimensionState;
	
	public void ReduceSetAppDimensionsAction(Func<AppDimensionState, AppDimensionState> withFunc)
	{
		var inState = GetAppDimensionState();
		
		_appDimensionState = withFunc.Invoke(inState);
		AppDimensionStateChanged?.Invoke();
		return;
	}

	public void ReduceNotifyIntraAppResizeAction()
	{
		AppDimensionStateChanged?.Invoke();
		return;
	}

	public void ReduceNotifyUserAgentResizeAction()
	{
		AppDimensionStateChanged?.Invoke();
		return;
	}
}

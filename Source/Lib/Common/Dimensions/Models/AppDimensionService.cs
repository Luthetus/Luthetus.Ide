namespace Luthetus.Common.RazorLib.Dimensions.Models;

public class AppDimensionService : IAppDimensionService
{
    private readonly object _stateModificationLock = new();

    private AppDimensionState _appDimensionState;
	
	public event Action? AppDimensionStateChanged;
	
	public AppDimensionState GetAppDimensionState() => _appDimensionState;
	
	public void SetAppDimensions(Func<AppDimensionState, AppDimensionState> withFunc)
	{
		lock (_stateModificationLock)
		{
			var inState = GetAppDimensionState();
			_appDimensionState = withFunc.Invoke(inState);
			goto finalize;
        }

		finalize:
        AppDimensionStateChanged?.Invoke();
    }

	public void NotifyIntraAppResize()
	{
		AppDimensionStateChanged?.Invoke();
    }

	public void NotifyUserAgentResize()
	{
		AppDimensionStateChanged?.Invoke();
    }
}

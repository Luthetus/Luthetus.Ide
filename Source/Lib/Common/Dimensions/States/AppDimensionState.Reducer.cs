using Fluxor;

namespace Luthetus.Common.RazorLib.Dimensions.States;

public partial record AppDimensionState
{
	public static class Reducer
	{
		[ReducerMethod]
		public static AppDimensionState ReduceSetAppDimensionsAction(
			AppDimensionState inState,
			SetAppDimensionStateAction setAppDimensionStateAction)
		{
			return setAppDimensionStateAction.WithFunc.Invoke(inState);
		}

		[ReducerMethod(typeof(NotifyIntraAppResizeAction))]
		public static AppDimensionState ReduceNotifyIntraAppResizeAction(
			AppDimensionState inState)
		{
			// Recreate the current state so the UI gets a state changed event
			return inState with {};
		}

		[ReducerMethod(typeof(NotifyUserAgentResizeAction))]
		public static AppDimensionState ReduceNotifyUserAgentResizeAction(
			AppDimensionState inState)
		{
			// Recreate the current state so the UI gets a state changed event
			return inState with {};
		}
	}
}

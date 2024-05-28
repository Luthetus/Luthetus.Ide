using Fluxor;

namespace Luthetus.Common.RazorLib.Dimensions.States;

public partial record AppDimensionState
{
	public static class Reducer
	{
		[ReducerMethod]
		public static AppDimensionState ReduceSetAppDimensionsAction(
			AppDimensionState inAppDimensionState,
			SetAppDimensionStateAction setAppDimensionStateAction)
		{
			return setAppDimensionStateAction.NextAppDimensionState;
		}
	}
}

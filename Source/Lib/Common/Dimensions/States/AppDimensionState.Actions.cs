namespace Luthetus.Common.RazorLib.Dimensions.States;

public partial record AppDimensionState
{
	public record SetAppDimensionStateAction(AppDimensionState NextAppDimensionState);
}

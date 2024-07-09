using Fluxor;

namespace Luthetus.Common.RazorLib.Outlines.States;

public partial record OutlineState
{
	public class Reducer
	{
		[ReducerMethod]
		public static OutlineState ReduceSetOutlineAction(
			OutlineState inState,
			SetOutlineAction setOutlineAction)
		{
			return inState with
			{
				ElementId = setOutlineAction.ElementId,
				MeasuredHtmlElementDimensions = setOutlineAction.MeasuredHtmlElementDimensions,
				NeedsMeasured = setOutlineAction.NeedsMeasured,
			};
		}
		
		[ReducerMethod]
		public static OutlineState ReduceSetMeasurementsAction(
			OutlineState inState,
			SetMeasurementsAction setMeasurementsAction)
		{
			if (inState.ElementId != setMeasurementsAction.ElementId)
				return inState;
				
			return inState with
			{
				MeasuredHtmlElementDimensions = setMeasurementsAction.MeasuredHtmlElementDimensions,
				NeedsMeasured = false,
			};
		}
	}
}

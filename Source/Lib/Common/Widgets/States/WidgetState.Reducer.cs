using Fluxor;

namespace Luthetus.Common.RazorLib.Widgets.States;

public partial record WidgetState
{
	public class Reducer
	{
		[ReducerMethod]
        public static WidgetState ReduceSetWidgetAction(
            WidgetState inState,
            SetWidgetAction setWidgetAction)
        {
        	if (setWidgetAction.Widget != inState.Widget)
        		setWidgetAction.ResultedInChange = true;
        
            return inState with 
            {
                Widget = setWidgetAction.Widget,
            };
        }
	}
}

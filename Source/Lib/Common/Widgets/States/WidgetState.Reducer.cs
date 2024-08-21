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
            return inState with 
            {
                Widget = setWidgetAction.Widget,
            };
        }
	}
}

using Fluxor;

namespace Luthetus.Common.RazorLib.Drags.Displays;

public partial record DragState
{
    public class Reducer
    {
        [ReducerMethod]
        public static DragState ReduceShouldDisplayAndMouseEventArgsSetAction(
            DragState inState,
            ShouldDisplayAndMouseEventArgsSetAction shouldDisplayAndMouseEventArgsSetAction)
        {
            return inState with
            {
            	ShouldDisplay = shouldDisplayAndMouseEventArgsSetAction.ShouldDisplay,
	            MouseEventArgs = shouldDisplayAndMouseEventArgsSetAction.MouseEventArgs,
            };
        }
        
        [ReducerMethod]
        public static DragState ReduceShouldDisplayAndMouseEventArgsAndDragSetAction(
            DragState inState,
            ShouldDisplayAndMouseEventArgsAndDragSetAction shouldDisplayAndMouseEventArgsAndDragSetAction)
        {
            return inState with
            {
            	ShouldDisplay = shouldDisplayAndMouseEventArgsAndDragSetAction.ShouldDisplay,
	            MouseEventArgs = shouldDisplayAndMouseEventArgsAndDragSetAction.MouseEventArgs,
	            Drag = shouldDisplayAndMouseEventArgsAndDragSetAction.Drag,
            };
        }
    }
}
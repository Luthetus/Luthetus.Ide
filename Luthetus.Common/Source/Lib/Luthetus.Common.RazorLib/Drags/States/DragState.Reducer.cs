using Fluxor;

namespace Luthetus.Common.RazorLib.Drags.Displays;

public partial record DragState
{
    private class Reducer
    {
        [ReducerMethod]
        public static DragState ReduceWithAction(
            DragState inState,
            WithAction withAction)
        {
            return withAction.WithFunc.Invoke(inState);
        }
    }
}
using Fluxor;

namespace Luthetus.Common.RazorLib.Options.States;

public partial record AppOptionsStateTests
{
    private record Reducer
    {
        [ReducerMethod]
        public static AppOptionsState ReduceWithAction(
            AppOptionsState inState,
            WithAction withAction)
        {
            return withAction.WithFunc.Invoke(inState);
        }
    }
}
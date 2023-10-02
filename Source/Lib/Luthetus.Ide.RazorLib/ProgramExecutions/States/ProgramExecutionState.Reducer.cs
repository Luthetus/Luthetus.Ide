using Fluxor;

namespace Luthetus.Ide.RazorLib.ProgramExecutions.States;

public partial record ProgramExecutionState
{
    private class Reducer
    {
        [ReducerMethod]
        public static ProgramExecutionState ReduceSetStartupProjectAbsolutePathAction(
            ProgramExecutionState inState,
            SetStartupProjectAbsolutePathAction setStartupProjectAbsolutePathAction)
        {
            return inState with
            {
                StartupProjectAbsolutePath = setStartupProjectAbsolutePathAction.StartupProjectAbsolutePath
            };
        }
    }
}
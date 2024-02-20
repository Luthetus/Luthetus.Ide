using Fluxor;

namespace Luthetus.Ide.Tests.Basis.ProgramExecutions.States;

public class ProgramExecutionStateReducerTests
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
using Fluxor;

namespace Luthetus.Ide.RazorLib.ProgramExecutionCase.States;

public partial record ProgramExecutionState
{
    private class Reducer
    {
        [ReducerMethod]
        public static ProgramExecutionState ReduceSetStartupProjectAbsolutePathAction(
            ProgramExecutionState inProgramExecutionState,
            ProgramExecutionState.SetStartupProjectAbsolutePathAction setStartupProjectAbsolutePathAction)
        {
            return inProgramExecutionState with
            {
                StartupProjectAbsolutePath =
                    setStartupProjectAbsolutePathAction.StartupProjectAbsolutePath
            };
        }
    }
}
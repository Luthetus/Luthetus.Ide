using Fluxor;

namespace Luthetus.Ide.RazorLib.ProgramExecutionCase.States;

public class ProgramExecutionRegistryReducer
{
    [ReducerMethod]
    public static ProgramExecutionRegistry ReduceSetStartupProjectAbsolutePathAction(
        ProgramExecutionRegistry inProgramExecutionState,
        ProgramExecutionRegistry.SetStartupProjectAbsolutePathAction setStartupProjectAbsolutePathAction)
    {
        return inProgramExecutionState with
        {
            StartupProjectAbsolutePath =
                setStartupProjectAbsolutePathAction.StartupProjectAbsolutePath
        };
    }
}
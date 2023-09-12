using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.ProgramExecutionCase;

public class ProgramExecutionRegistryReducer
{
    [ReducerMethod]
    public static ProgramExecutionRegistry ReduceSetStartupProjectAbsoluteFilePathAction(
        ProgramExecutionRegistry inProgramExecutionState,
        ProgramExecutionRegistry.SetStartupProjectAbsoluteFilePathAction setStartupProjectAbsoluteFilePathAction)
    {
        return inProgramExecutionState with
        {
            StartupProjectAbsoluteFilePath =
                setStartupProjectAbsoluteFilePathAction.StartupProjectAbsoluteFilePath
        };
    }
}
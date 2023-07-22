using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.ProgramExecutionCase;

[FeatureState]
public record ProgramExecutionState(IAbsoluteFilePath? StartupProjectAbsoluteFilePath)
{
    private ProgramExecutionState() : this(default(IAbsoluteFilePath))
    {

    }

    public record SetStartupProjectAbsoluteFilePathAction(IAbsoluteFilePath? StartupProjectAbsoluteFilePath);
}
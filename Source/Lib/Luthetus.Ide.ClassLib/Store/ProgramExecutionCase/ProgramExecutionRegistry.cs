using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.ProgramExecutionCase;

[FeatureState]
public record ProgramExecutionRegistry(IAbsoluteFilePath? StartupProjectAbsoluteFilePath)
{
    private ProgramExecutionRegistry() : this(default(IAbsoluteFilePath))
    {

    }

    public record SetStartupProjectAbsoluteFilePathAction(IAbsoluteFilePath? StartupProjectAbsoluteFilePath);
}
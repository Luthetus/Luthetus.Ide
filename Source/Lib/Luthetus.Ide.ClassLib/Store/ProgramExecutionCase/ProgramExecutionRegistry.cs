using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.ProgramExecutionCase;

[FeatureState]
public record ProgramExecutionRegistry(IAbsolutePath? StartupProjectAbsoluteFilePath)
{
    private ProgramExecutionRegistry() : this(default(IAbsolutePath))
    {

    }

    public record SetStartupProjectAbsoluteFilePathAction(IAbsolutePath? StartupProjectAbsoluteFilePath);
}
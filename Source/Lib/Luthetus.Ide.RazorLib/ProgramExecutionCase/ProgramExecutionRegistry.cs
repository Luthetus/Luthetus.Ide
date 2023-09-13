using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.ProgramExecutionCase;

[FeatureState]
public record ProgramExecutionRegistry(IAbsolutePath? StartupProjectAbsolutePath)
{
    private ProgramExecutionRegistry() : this(default(IAbsolutePath))
    {

    }

    public record SetStartupProjectAbsolutePathAction(IAbsolutePath? StartupProjectAbsolutePath);
}
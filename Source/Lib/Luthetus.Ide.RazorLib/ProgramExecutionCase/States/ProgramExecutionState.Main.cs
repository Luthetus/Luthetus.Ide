using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Models;

namespace Luthetus.Ide.RazorLib.ProgramExecutionCase.States;

[FeatureState]
public partial record ProgramExecutionState(IAbsolutePath? StartupProjectAbsolutePath)
{
    private ProgramExecutionState() : this(default(IAbsolutePath))
    {

    }

    public record SetStartupProjectAbsolutePathAction(IAbsolutePath? StartupProjectAbsolutePath);
}
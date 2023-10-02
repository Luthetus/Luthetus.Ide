using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.ProgramExecutions.States;

[FeatureState]
public partial record ProgramExecutionState(IAbsolutePath? StartupProjectAbsolutePath)
{
    private ProgramExecutionState() : this(default(IAbsolutePath))
    {

    }
}
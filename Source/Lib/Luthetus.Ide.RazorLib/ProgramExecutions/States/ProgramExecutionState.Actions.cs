using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.ProgramExecutions.States;

public partial record ProgramExecutionState
{
    public record SetStartupProjectAbsolutePathAction(IAbsolutePath? StartupProjectAbsolutePath);
}
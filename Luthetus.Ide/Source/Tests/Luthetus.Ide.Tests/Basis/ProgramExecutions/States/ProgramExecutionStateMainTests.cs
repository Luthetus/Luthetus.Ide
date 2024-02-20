using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.Tests.Basis.ProgramExecutions.States;

public class ProgramExecutionStateMainTests(IAbsolutePath? StartupProjectAbsolutePath)
{
    private ProgramExecutionState() : this(default(IAbsolutePath))
    {

    }
}
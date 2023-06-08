using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;

public partial record DotNetSolutionState
{
    public record SetDotNetSolutionAction(IAbsoluteFilePath SolutionAbsoluteFilePath);

    private record WithAction(Func<DotNetSolutionState, DotNetSolutionState> WithFunc);
    private record SetDotNetSolutionTreeViewAction;
}
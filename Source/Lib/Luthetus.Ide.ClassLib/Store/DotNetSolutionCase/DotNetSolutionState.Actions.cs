using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;

public partial record DotNetSolutionState
{
    public record SetDotNetSolutionAction(IAbsoluteFilePath SolutionAbsoluteFilePath);
    public record WithAction(Func<DotNetSolutionState, DotNetSolutionState> WithFunc);
    public record SetDotNetSolutionTreeViewAction;

    private record ParseDotNetSolutionAction;
}
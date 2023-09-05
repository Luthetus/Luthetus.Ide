using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;

namespace Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;

public partial record DotNetSolutionState
{
    public record RegisterAction(DotNetSolutionModel DotNetSolutionModel);

    public record DisposeAction(DotNetSolutionModelKey DotNetSolutionModelKey);

    public record SetDotNetSolutionAction(IAbsoluteFilePath SolutionAbsoluteFilePath);

    public record WithAction(Func<DotNetSolutionState, DotNetSolutionState> WithFunc);

    public record SetDotNetSolutionTreeViewAction;

    public record AddExistingProjectToSolutionAction(
        DotNetSolutionModelKey DotNetSolutionModelKey,
        string LocalProjectTemplateShortName,
        string LocalCSharpProjectName,
        IAbsoluteFilePath CSharpProjectAbsoluteFilePath,
        IEnvironmentProvider EnvironmentProvider);

    private record ParseDotNetSolutionAction;
}
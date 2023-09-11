using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;

namespace Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;

public partial record DotNetSolutionRegistry
{
    public record RegisterAction(DotNetSolutionModel DotNetSolutionModel);

    public record DisposeAction(DotNetSolutionModelKey DotNetSolutionModelKey);

    public record SetDotNetSolutionAction(IAbsoluteFilePath SolutionAbsoluteFilePath);

    public record WithAction(Func<DotNetSolutionRegistry, DotNetSolutionRegistry> WithFunc);

    public record SetDotNetSolutionTreeViewAction;

    public record AddExistingProjectToSolutionAction(
        DotNetSolutionModelKey DotNetSolutionModelKey,
        string LocalProjectTemplateShortName,
        string LocalCSharpProjectName,
        IAbsoluteFilePath CSharpProjectAbsoluteFilePath,
        IEnvironmentProvider EnvironmentProvider);

    private record ParseDotNetSolutionAction;
}
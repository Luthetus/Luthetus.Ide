using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

public partial record DotNetSolutionState
{
    public record RegisterAction(DotNetSolutionModel DotNetSolutionModel, DotNetSolutionSync Sync);
    public record DisposeAction(DotNetSolutionModelKey DotNetSolutionModelKey, DotNetSolutionSync Sync);
    public record SetDotNetSolutionTask(IAbsolutePath SolutionAbsolutePath, DotNetSolutionSync Sync);
    public record SetDotNetSolutionTreeViewTask(DotNetSolutionModelKey DotNetSolutionModelKey, DotNetSolutionSync Sync);

    public record AddExistingProjectToSolutionTask(
        DotNetSolutionModelKey DotNetSolutionModelKey,
        string LocalProjectTemplateShortName,
        string LocalCSharpProjectName,
        IAbsolutePath CSharpProjectAbsolutePath,
        IEnvironmentProvider EnvironmentProvider,
        DotNetSolutionSync Sync);
}
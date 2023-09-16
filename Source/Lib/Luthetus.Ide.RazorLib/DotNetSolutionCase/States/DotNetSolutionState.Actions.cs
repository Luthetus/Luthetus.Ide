using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.KeyCase;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

public partial record DotNetSolutionState
{
    public record RegisterAction(DotNetSolutionModel DotNetSolutionModel, DotNetSolutionSync Sync);
    public record DisposeAction(Key<DotNetSolutionModel> DotNetSolutionModelKey, DotNetSolutionSync Sync);
    public record SetDotNetSolutionTask(IAbsolutePath SolutionAbsolutePath, DotNetSolutionSync Sync);
    public record SetDotNetSolutionTreeViewTask(Key<DotNetSolutionModel> DotNetSolutionModelKey, DotNetSolutionSync Sync);

    public record AddExistingProjectToSolutionTask(
        Key<DotNetSolutionModel> DotNetSolutionModelKey,
        string LocalProjectTemplateShortName,
        string LocalCSharpProjectName,
        IAbsolutePath CSharpProjectAbsolutePath,
        IEnvironmentProvider EnvironmentProvider,
        DotNetSolutionSync Sync);
}
using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Ide.RazorLib.FileSystemCase;
using Luthetus.Ide.RazorLib.InputFileCase;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;

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

    public static void ShowInputFile(DotNetSolutionSync sync)
    {
        sync.Dispatcher.Dispatch(new InputFileRegistry.RequestInputFileStateFormAction(
            "Solution Explorer",
            afp =>
            {
                if (afp is not null)
                    sync.Dispatcher.Dispatch(new SetDotNetSolutionTask(afp, sync));

                return Task.CompletedTask;
            },
            afp =>
            {
                if (afp is null || afp.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(
                    afp.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION);
            },
            new[]
            {
                new InputFilePattern(
                    ".NET Solution",
                    afp => afp.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
            }.ToImmutableArray()));
    }
}
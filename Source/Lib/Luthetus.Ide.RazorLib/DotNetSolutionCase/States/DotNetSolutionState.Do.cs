using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Ide.RazorLib.FileSystemCase;
using Luthetus.Ide.RazorLib.InputFileCase;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

public partial record DotNetSolutionState
{
    public record RegisterAction(DotNetSolutionModel DotNetSolutionModel);
    public record DisposeAction(DotNetSolutionModelKey DotNetSolutionModelKey);
    public record SetDotNetSolutionTask(IAbsolutePath SolutionAbsolutePath);
    public record SetDotNetSolutionTreeViewTask(DotNetSolutionModelKey DotNetSolutionModelKey);

    public record AddExistingProjectToSolutionTask(
        DotNetSolutionModelKey DotNetSolutionModelKey,
        string LocalProjectTemplateShortName,
        string LocalCSharpProjectName,
        IAbsolutePath CSharpProjectAbsolutePath,
        IEnvironmentProvider EnvironmentProvider);

    public record ParseDotNetSolutionTask;

    public static void ShowInputFile(IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new InputFileRegistry.RequestInputFileStateFormAction(
            "Solution Explorer",
            afp =>
            {
                if (afp is not null)
                    dispatcher.Dispatch(new SetDotNetSolutionTask(afp));

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
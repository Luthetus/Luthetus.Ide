using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Ide.RazorLib.FileSystemCase;
using Luthetus.Ide.RazorLib.InputFileCase;
using Luthetus.Ide.ClassLib.Store.InputFileCase;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase;

/// <param name="IsExecutingAsyncTaskLinks">Each link is a separate async task. The async task increments the links when it starts, then decrements when it is finished. A loading icon is shown in the solution explorer if links > 0</param>
[FeatureState]
public partial record DotNetSolutionRegistry(
    DotNetSolutionModelKey? DotNetSolutionModelKey,
    int IsExecutingAsyncTaskLinks)
{
    public static readonly TreeViewStateKey TreeViewSolutionExplorerStateKey = TreeViewStateKey.NewKey();

    private DotNetSolutionRegistry() : this(
        DotNetSolutionModelKey.Empty,
        0)
    {
        DotNetSolutions = ImmutableList<DotNetSolutionModel>.Empty;
    }

    public ImmutableList<DotNetSolutionModel> DotNetSolutions { get; init; }

    public DotNetSolutionModel? DotNetSolutionModel => DotNetSolutions.FirstOrDefault(x =>
        x.DotNetSolutionModelKey == DotNetSolutionModelKey);

    public static void ShowInputFile(
        IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new InputFileRegistry.RequestInputFileStateFormAction(
            "Solution Explorer",
            afp =>
            {
                if (afp is not null)
                    dispatcher.Dispatch(new SetDotNetSolutionAction(afp));

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
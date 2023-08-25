using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.Store.InputFileCase;
using Luthetus.Ide.ClassLib.FileConstants;
using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.DotNetSolution;

namespace Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;

/// <param name="IsExecutingAsyncTaskLinks">Each link is a separate async task. The async task increments the links when it starts, then decrements when it is finished. A loading icon is shown in the solution explorer if links > 0</param>
[FeatureState]
public partial record DotNetSolutionState(
    DotNetSolution? DotNetSolution,
    int IsExecutingAsyncTaskLinks)
{
    public static readonly TreeViewStateKey TreeViewSolutionExplorerStateKey = TreeViewStateKey.NewTreeViewStateKey();

    private DotNetSolutionState() : this(
        default(DotNetSolution?),
        0)
    {
    }

    public static void ShowInputFile(
        IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new InputFileState.RequestInputFileStateFormAction(
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
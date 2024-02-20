using Fluxor;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;

namespace Luthetus.Ide.Tests.Basis.DotNetSolutions.States;

public class DotNetSolutionStateMainTests(
    Key<DotNetSolutionModel>? DotNetSolutionModelKey,
    int IsExecutingAsyncTaskLinks)
{
    public static readonly Key<TreeViewContainer> TreeViewSolutionExplorerStateKey = Key<TreeViewContainer>.NewKey();

    private DotNetSolutionState() : this(Key<DotNetSolutionModel>.Empty, 0)
    {
    }

    public ImmutableList<DotNetSolutionModel> DotNetSolutionsList { get; init; } = ImmutableList<DotNetSolutionModel>.Empty;

    public DotNetSolutionModel? DotNetSolutionModel => DotNetSolutionsList.FirstOrDefault(x =>
        x.Key == DotNetSolutionModelKey);

    public static void ShowInputFile(DotNetSolutionSync sync)
    {
        sync.InputFileSync.RequestInputFileStateForm("Solution Explorer",
            afp =>
            {
                if (afp is not null)
                    sync.SetDotNetSolution(afp);

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
            }.ToImmutableArray());
    }
}
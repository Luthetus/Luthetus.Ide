using Fluxor;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.RewriteForImmutability;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.States;

/// <summary>
/// Only allow ImmutableEntry replacement calculations via DotNetSolutionSync.Tasks.cs.<br/><br/>
/// Only allow ImmutableList&lt;ImmutableEntry&gt; replacements via DotNetSolutionSync.Actions.cs<br/><br/>
/// If one follows this pattern, then calculation of a given Entry's replacement will be
/// performed within a shared "async-concurrent context".<br/><br/>
/// Furthermore, List&lt;Entry&gt; replacement will be performed within a shared "synchronous-concurrent context".<br/><br/>
/// When accessing states, one has to be pessimistic. One might access an ImmutableEntry from the
/// ImmutableList, then perform the asynchronous calculation, and ultimately find that the original
/// ImmutableEntry no longer exists in the ImmutableList. This behavior is intended.
/// </summary>
[FeatureState]
public partial record DotNetSolutionState(
    Key<DotNetSolutionModel>? DotNetSolutionModelKey,
    int IsExecutingAsyncTaskLinks)
{
    public static readonly Key<TreeViewContainer> TreeViewSolutionExplorerStateKey = Key<TreeViewContainer>.NewKey();

    private DotNetSolutionState() : this(Key<DotNetSolutionModel>.Empty, 0)
    {
    }

    public ImmutableList<DotNetSolutionModel> DotNetSolutionsBag { get; init; } = ImmutableList<DotNetSolutionModel>.Empty;

    public DotNetSolutionModel? DotNetSolutionModel => DotNetSolutionsBag.FirstOrDefault(x =>
        x.DotNetSolutionModelKey == DotNetSolutionModelKey);

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
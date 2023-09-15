using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Ide.RazorLib.InputFileCase;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Ide.RazorLib.FileSystemCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.States;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

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
    DotNetSolutionModelKey? DotNetSolutionModelKey,
    int IsExecutingAsyncTaskLinks)
{
    public static readonly TreeViewStateKey TreeViewSolutionExplorerStateKey = TreeViewStateKey.NewKey();

    private DotNetSolutionState() : this(DotNetSolutionModelKey.Empty, 0)
    {
    }

    public ImmutableList<DotNetSolutionModel> DotNetSolutions { get; init; } = ImmutableList<DotNetSolutionModel>.Empty;

    public DotNetSolutionModel? DotNetSolutionModel => DotNetSolutions.FirstOrDefault(x =>
        x.DotNetSolutionModelKey == DotNetSolutionModelKey);

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
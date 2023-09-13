using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Ide.RazorLib.FileSystemCase;
using Luthetus.Ide.RazorLib.InputFileCase;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

/// <param name="IsExecutingAsyncTaskLinks">Each link is a separate async task. The async task increments the links when it starts, then decrements when it is finished. A loading icon is shown in the solution explorer if links > 0</param>
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
}
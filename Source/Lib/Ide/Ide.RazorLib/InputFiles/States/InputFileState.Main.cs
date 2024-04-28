using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.InputFiles.States;

[FeatureState]
public partial record InputFileState(
    int IndexInHistory,
    ImmutableList<TreeViewAbsolutePath> OpenedTreeViewModelHistoryList,
    TreeViewAbsolutePath? SelectedTreeViewModel,
    Func<IAbsolutePath?, Task> OnAfterSubmitFunc,
    Func<IAbsolutePath?, Task<bool>> SelectionIsValidFunc,
    ImmutableArray<InputFilePattern> InputFilePatternsList,
    InputFilePattern? SelectedInputFilePattern,
    string SearchQuery,
    string Message)
{
    private InputFileState() : this(
        -1,
        ImmutableList<TreeViewAbsolutePath>.Empty,
        null,
        _ => Task.CompletedTask,
        _ => Task.FromResult(false),
        ImmutableArray<InputFilePattern>.Empty,
        null,
        string.Empty,
        string.Empty)
    {
    }
}
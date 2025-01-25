using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.States;

[FeatureState]
public partial record InputFileState(
    int IndexInHistory,
    ImmutableList<TreeViewAbsolutePath> OpenedTreeViewModelHistoryList,
    TreeViewAbsolutePath? SelectedTreeViewModel,
    Func<AbsolutePath, Task> OnAfterSubmitFunc,
    Func<AbsolutePath, Task<bool>> SelectionIsValidFunc,
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
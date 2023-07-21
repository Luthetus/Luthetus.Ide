using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.TreeViewImplementations;

namespace Luthetus.Ide.ClassLib.Store.InputFileCase;

[FeatureState]
public partial record InputFileState(
    int IndexInHistory,
    ImmutableList<TreeViewAbsoluteFilePath> OpenedTreeViewModelHistory,
    TreeViewAbsoluteFilePath? SelectedTreeViewModel,
    Func<IAbsoluteFilePath?, Task> OnAfterSubmitFunc,
    Func<IAbsoluteFilePath?, Task<bool>> SelectionIsValidFunc,
    ImmutableArray<InputFilePattern> InputFilePatterns,
    InputFilePattern? SelectedInputFilePattern,
    string SearchQuery,
    string Message)
{
    private InputFileState() : this(
        -1,
        ImmutableList<TreeViewAbsoluteFilePath>.Empty,
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
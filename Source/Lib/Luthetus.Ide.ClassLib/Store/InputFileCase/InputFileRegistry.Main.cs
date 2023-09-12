using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Store.InputFileCase;

[FeatureState]
public partial record InputFileRegistry(
    int IndexInHistory,
    ImmutableList<TreeViewAbsoluteFilePath> OpenedTreeViewModelHistory,
    TreeViewAbsoluteFilePath? SelectedTreeViewModel,
    Func<IAbsolutePath?, Task> OnAfterSubmitFunc,
    Func<IAbsolutePath?, Task<bool>> SelectionIsValidFunc,
    ImmutableArray<InputFilePattern> InputFilePatterns,
    InputFilePattern? SelectedInputFilePattern,
    string SearchQuery,
    string Message)
{
    private InputFileRegistry() : this(
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
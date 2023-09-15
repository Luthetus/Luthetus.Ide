using Fluxor;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.InputFileCase.States;

[FeatureState]
public partial record InputFileRegistry(
    int IndexInHistory,
    ImmutableList<TreeViewAbsolutePath> OpenedTreeViewModelHistory,
    TreeViewAbsolutePath? SelectedTreeViewModel,
    Func<IAbsolutePath?, Task> OnAfterSubmitFunc,
    Func<IAbsolutePath?, Task<bool>> SelectionIsValidFunc,
    ImmutableArray<InputFilePattern> InputFilePatterns,
    InputFilePattern? SelectedInputFilePattern,
    string SearchQuery,
    string Message)
{
    private InputFileRegistry() : this(
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
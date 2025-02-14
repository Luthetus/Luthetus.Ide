using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Models;

public record struct InputFileState(
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
    public InputFileState() : this(
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
    
    public bool CanMoveBackwardsInHistory => IndexInHistory > 0;
    public bool CanMoveForwardsInHistory => IndexInHistory < OpenedTreeViewModelHistoryList.Count - 1;

    public TreeViewAbsolutePath? GetOpenedTreeView()
    {
        if (IndexInHistory == -1 || IndexInHistory >= OpenedTreeViewModelHistoryList.Count)
            return null;

        return OpenedTreeViewModelHistoryList[IndexInHistory];
    }

    public static InputFileState NewOpenedTreeViewModelHistory(
        InputFileState inInputFileState,
        TreeViewAbsolutePath selectedTreeViewModel,
        IIdeComponentRenderers ideComponentRenderers,
        LuthetusCommonApi commonApi)
    {
        var selectionClone = new TreeViewAbsolutePath(
            selectedTreeViewModel.Item,
            commonApi,
            ideComponentRenderers,
            false,
            true);

        selectionClone.IsExpanded = true;
        selectionClone.ChildList = selectedTreeViewModel.ChildList;
        
        var nextHistory = inInputFileState.OpenedTreeViewModelHistoryList;

        // If not at end of history the more recent history is
        // replaced by the to be selected TreeViewModel
        if (inInputFileState.IndexInHistory != inInputFileState.OpenedTreeViewModelHistoryList.Count - 1)
        {
            var historyCount = inInputFileState.OpenedTreeViewModelHistoryList.Count;
            var startingIndexToRemove = inInputFileState.IndexInHistory + 1;
            var countToRemove = historyCount - startingIndexToRemove;

            nextHistory = inInputFileState.OpenedTreeViewModelHistoryList.RemoveRange(
                startingIndexToRemove,
                countToRemove);
        }

        nextHistory = nextHistory.Add(selectionClone);

        return inInputFileState with
        {
            IndexInHistory = inInputFileState.IndexInHistory + 1,
            OpenedTreeViewModelHistoryList = nextHistory,
        };
    }
}
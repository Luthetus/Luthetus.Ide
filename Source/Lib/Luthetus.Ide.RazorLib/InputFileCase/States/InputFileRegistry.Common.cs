using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;

namespace Luthetus.Ide.RazorLib.InputFileCase.States;

public partial record InputFileRegistry
{
    public bool CanMoveBackwardsInHistory => IndexInHistory > 0;

    public bool CanMoveForwardsInHistory => IndexInHistory <
        OpenedTreeViewModelHistory.Count - 1;

    public TreeViewAbsolutePath? GetOpenedTreeView()
    {
        if (IndexInHistory == -1 ||
            IndexInHistory >= OpenedTreeViewModelHistory.Count)
            return null;

        return OpenedTreeViewModelHistory[IndexInHistory];
    }

    private static InputFileRegistry NewOpenedTreeViewModelHistory(
        InputFileRegistry inInputFileState,
        TreeViewAbsolutePath selectedTreeViewModel,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var selectionClone = new TreeViewAbsolutePath(
            selectedTreeViewModel.Item,
            luthetusIdeComponentRenderers,
            luthetusCommonComponentRenderers,
            fileSystemProvider,
            environmentProvider,
            false,
            true);

        selectionClone.IsExpanded = true;

        selectionClone.Children = selectedTreeViewModel.Children;

        var nextHistory =
            inInputFileState.OpenedTreeViewModelHistory;

        // If not at end of history the more recent history is
        // replaced by the to be selected TreeViewModel
        if (inInputFileState.IndexInHistory !=
            inInputFileState.OpenedTreeViewModelHistory.Count - 1)
        {
            var historyCount = inInputFileState.OpenedTreeViewModelHistory.Count;
            var startingIndexToRemove = inInputFileState.IndexInHistory + 1;
            var countToRemove = historyCount - startingIndexToRemove;

            nextHistory = inInputFileState.OpenedTreeViewModelHistory
                .RemoveRange(startingIndexToRemove, countToRemove);
        }

        nextHistory = nextHistory
            .Add(selectionClone);

        return inInputFileState with
        {
            IndexInHistory = inInputFileState.IndexInHistory + 1,
            OpenedTreeViewModelHistory = nextHistory,
        };
    }
}
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.States;

public partial record InputFileState
{
    public bool CanMoveBackwardsInHistory => IndexInHistory > 0;
    public bool CanMoveForwardsInHistory => IndexInHistory < OpenedTreeViewModelHistoryBag.Count - 1;

    public TreeViewAbsolutePath? GetOpenedTreeView()
    {
        if (IndexInHistory == -1 || IndexInHistory >= OpenedTreeViewModelHistoryBag.Count)
            return null;

        return OpenedTreeViewModelHistoryBag[IndexInHistory];
    }

    private static InputFileState NewOpenedTreeViewModelHistory(
        InputFileState inInputFileState,
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
        selectionClone.ChildBag = selectedTreeViewModel.ChildBag;
        
        var nextHistory = inInputFileState.OpenedTreeViewModelHistoryBag;

        // If not at end of history the more recent history is
        // replaced by the to be selected TreeViewModel
        if (inInputFileState.IndexInHistory != inInputFileState.OpenedTreeViewModelHistoryBag.Count - 1)
        {
            var historyCount = inInputFileState.OpenedTreeViewModelHistoryBag.Count;
            var startingIndexToRemove = inInputFileState.IndexInHistory + 1;
            var countToRemove = historyCount - startingIndexToRemove;

            nextHistory = inInputFileState.OpenedTreeViewModelHistoryBag.RemoveRange(
                startingIndexToRemove,
                countToRemove);
        }

        nextHistory = nextHistory.Add(selectionClone);

        return inInputFileState with
        {
            IndexInHistory = inInputFileState.IndexInHistory + 1,
            OpenedTreeViewModelHistoryBag = nextHistory,
        };
    }
}
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.Tests.Basis.InputFiles.States;

public class InputFileStateCommonTests
{
    public bool CanMoveBackwardsInHistory => IndexInHistory > 0;
    public bool CanMoveForwardsInHistory => IndexInHistory < OpenedTreeViewModelHistoryList.Count - 1;

    public TreeViewAbsolutePath? GetOpenedTreeView()
    {
        if (IndexInHistory == -1 || IndexInHistory >= OpenedTreeViewModelHistoryList.Count)
            return null;

        return OpenedTreeViewModelHistoryList[IndexInHistory];
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
using Fluxor;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.States;

public partial record InputFileState
{
    public static class Reducer
    {
        [ReducerMethod]
        public static InputFileState ReduceStartInputFileStateFormAction(
            InputFileState inState,
            StartInputFileStateFormAction startInputFileStateFormAction)
        {
            return inState with
            {
                SelectionIsValidFunc = startInputFileStateFormAction.SelectionIsValidFunc,
                OnAfterSubmitFunc = startInputFileStateFormAction.OnAfterSubmitFunc,
                InputFilePatternsList = startInputFileStateFormAction.InputFilePatterns,
                SelectedInputFilePattern = startInputFileStateFormAction.InputFilePatterns.First(),
                Message = startInputFileStateFormAction.Message
            };
        }

        [ReducerMethod]
        public static InputFileState ReduceSetSelectedTreeViewModelAction(
            InputFileState inState,
            SetSelectedTreeViewModelAction setSelectedTreeViewModelAction)
        {
            return inState with
            {
                SelectedTreeViewModel = setSelectedTreeViewModelAction.SelectedTreeViewModel
            };
        }

        [ReducerMethod]
        public static InputFileState ReduceSetOpenedTreeViewModelAction(
            InputFileState inState,
            SetOpenedTreeViewModelAction setOpenedTreeViewModelAction)
        {
            if (setOpenedTreeViewModelAction.TreeViewModel.Item.IsDirectory)
            {
                return NewOpenedTreeViewModelHistory(
                    inState,
                    setOpenedTreeViewModelAction.TreeViewModel,
                    setOpenedTreeViewModelAction.IdeComponentRenderers,
                    setOpenedTreeViewModelAction.CommonComponentRenderers,
                    setOpenedTreeViewModelAction.FileSystemProvider,
                    setOpenedTreeViewModelAction.EnvironmentProvider);
            }

            return inState;
        }

        [ReducerMethod]
        public static InputFileState ReduceSetSelectedInputFilePatternAction(
            InputFileState inState,
            SetSelectedInputFilePatternAction setSelectedInputFilePatternAction)
        {
            return inState with
            {
                SelectedInputFilePattern = setSelectedInputFilePatternAction.InputFilePattern
            };
        }

        [ReducerMethod(typeof(MoveBackwardsInHistoryAction))]
        public static InputFileState ReduceMoveBackwardsInHistoryAction(InputFileState inState)
        {
            if (inState.CanMoveBackwardsInHistory)
                return inState with { IndexInHistory = inState.IndexInHistory - 1 };

            return inState;
        }

        [ReducerMethod(typeof(MoveForwardsInHistoryAction))]
        public static InputFileState ReduceMoveForwardsInHistoryAction(InputFileState inState)
        {
            if (inState.CanMoveForwardsInHistory)
                return inState with { IndexInHistory = inState.IndexInHistory + 1 };

            return inState;
        }

        [ReducerMethod]
        public static InputFileState ReduceOpenParentDirectoryAction(
            InputFileState inState,
            OpenParentDirectoryAction openParentDirectoryAction)
        {
            var currentSelection = inState.OpenedTreeViewModelHistoryList[inState.IndexInHistory];

            TreeViewAbsolutePath? parentDirectoryTreeViewModel = null;

            // If has a ParentDirectory select it
            if (currentSelection.Item.ParentDirectory is not null)
            {
                var parentDirectory = currentSelection.Item.ParentDirectory;

                var parentDirectoryAbsolutePath = openParentDirectoryAction.EnvironmentProvider.AbsolutePathFactory(
                    parentDirectory.Value,
                    true);

                parentDirectoryTreeViewModel = new TreeViewAbsolutePath(
                    parentDirectoryAbsolutePath,
                    openParentDirectoryAction.IdeComponentRenderers,
                    openParentDirectoryAction.CommonComponentRenderers,
                    openParentDirectoryAction.FileSystemProvider,
                    openParentDirectoryAction.EnvironmentProvider,
                    false,
                    true);

                openParentDirectoryAction.ParentDirectoryTreeViewModel = parentDirectoryTreeViewModel;
            }

            if (parentDirectoryTreeViewModel is not null)
            {
                return NewOpenedTreeViewModelHistory(
                    inState,
                    parentDirectoryTreeViewModel,
                    openParentDirectoryAction.IdeComponentRenderers,
                    openParentDirectoryAction.CommonComponentRenderers,
                    openParentDirectoryAction.FileSystemProvider,
                    openParentDirectoryAction.EnvironmentProvider);
            }

            return inState;
        }

        [ReducerMethod]
        public static InputFileState ReduceRefreshCurrentSelectionAction(
            InputFileState inState,
            RefreshCurrentSelectionAction refreshCurrentSelectionAction)
        {
            refreshCurrentSelectionAction.CurrentSelection = inState.OpenedTreeViewModelHistoryList[inState.IndexInHistory];
            return inState;
        }

        [ReducerMethod]
        public static InputFileState ReduceSetSearchQueryAction(
            InputFileState inState,
            SetSearchQueryAction setSearchQueryAction)
        {
            var openedTreeViewModel = inState.OpenedTreeViewModelHistoryList[inState.IndexInHistory];

            foreach (var treeViewModel in openedTreeViewModel.ChildList)
            {
                var treeViewAbsolutePath = (TreeViewAbsolutePath)treeViewModel;

                treeViewModel.IsHidden = !treeViewAbsolutePath.Item.NameWithExtension.Contains(
                    setSearchQueryAction.SearchQuery,
                    StringComparison.InvariantCultureIgnoreCase);
            }

            return inState with { SearchQuery = setSearchQueryAction.SearchQuery };
        }
    }
}
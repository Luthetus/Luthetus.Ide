using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase;

namespace Luthetus.Ide.ClassLib.Store.InputFileCase;

public partial record InputFileRegistry
{
    private static class Reducer
    {
        [ReducerMethod]
        public static InputFileRegistry ReduceStartInputFileStateFormAction(
            InputFileRegistry inInputFileState,
            StartInputFileStateFormAction startInputFileStateFormAction)
        {
            return inInputFileState with
            {
                SelectionIsValidFunc = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.SelectionIsValidFunc,
                OnAfterSubmitFunc = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.OnAfterSubmitFunc,
                InputFilePatterns = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.InputFilePatterns,
                SelectedInputFilePattern = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.InputFilePatterns
                    .First(),
                Message = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.Message
            };
        }

        [ReducerMethod]
        public static InputFileRegistry ReduceSetSelectedTreeViewModelAction(
            InputFileRegistry inInputFileState,
            SetSelectedTreeViewModelAction setSelectedTreeViewModelAction)
        {
            return inInputFileState with
            {
                SelectedTreeViewModel =
                    setSelectedTreeViewModelAction.SelectedTreeViewModel
            };
        }

        [ReducerMethod]
        public static InputFileRegistry ReduceSetOpenedTreeViewModelAction(
            InputFileRegistry inInputFileState,
            SetOpenedTreeViewModelAction setOpenedTreeViewModelAction)
        {
            if (setOpenedTreeViewModelAction.TreeViewModel.Item.IsDirectory)
            {
                return NewOpenedTreeViewModelHistory(
                    inInputFileState,
                    setOpenedTreeViewModelAction.TreeViewModel,
                    setOpenedTreeViewModelAction.LuthetusIdeComponentRenderers,
                    setOpenedTreeViewModelAction.LuthetusCommonComponentRenderers,
                    setOpenedTreeViewModelAction.FileSystemProvider,
                    setOpenedTreeViewModelAction.EnvironmentProvider);
            }

            return inInputFileState;
        }

        [ReducerMethod]
        public static InputFileRegistry ReduceSetSelectedInputFilePatternAction(
            InputFileRegistry inInputFileState,
            SetSelectedInputFilePatternAction setSelectedInputFilePatternAction)
        {
            return inInputFileState with
            {
                SelectedInputFilePattern =
                    setSelectedInputFilePatternAction.InputFilePattern
            };
        }

        [ReducerMethod]
        public static InputFileRegistry ReduceMoveBackwardsInHistoryAction(
            InputFileRegistry inInputFileState,
            MoveBackwardsInHistoryAction moveBackwardsInHistoryAction)
        {
            if (inInputFileState.CanMoveBackwardsInHistory)
            {
                return inInputFileState with
                {
                    IndexInHistory = inInputFileState.IndexInHistory -
                                             1,
                };
            }

            return inInputFileState;
        }

        [ReducerMethod]
        public static InputFileRegistry ReduceMoveForwardsInHistoryAction(
            InputFileRegistry inInputFileState,
            MoveForwardsInHistoryAction moveForwardsInHistoryAction)
        {
            if (inInputFileState.CanMoveForwardsInHistory)
            {
                return inInputFileState with
                {
                    IndexInHistory = inInputFileState.IndexInHistory +
                                             1,
                };
            }

            return inInputFileState;
        }

        [ReducerMethod]
        public static InputFileRegistry ReduceOpenParentDirectoryAction(
            InputFileRegistry inInputFileState,
            OpenParentDirectoryAction openParentDirectoryAction)
        {
            var currentSelection = inInputFileState
                .OpenedTreeViewModelHistory[inInputFileState.IndexInHistory];

            TreeViewAbsolutePath? parentDirectoryTreeViewModel = null;

            // If has a ParentDirectory select it
            if (currentSelection.Item.AncestorDirectories.Any())
            {
                var parentDirectoryAbsolutePath =
                    currentSelection.Item.AncestorDirectories.Last();

                parentDirectoryTreeViewModel = new TreeViewAbsolutePath(
                    (IAbsolutePath)parentDirectoryAbsolutePath,
                    openParentDirectoryAction.LuthetusIdeComponentRenderers,
                    openParentDirectoryAction.LuthetusCommonComponentRenderers,
                    openParentDirectoryAction.FileSystemProvider,
                    openParentDirectoryAction.EnvironmentProvider,
                    false,
                    true);

                openParentDirectoryAction.BackgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), CommonBackgroundTaskWorker.Queue.Key,
                    "Open Parent Directory",
                    async () =>
                    {
                        await parentDirectoryTreeViewModel.LoadChildrenAsync();
                    });
            }

            if (parentDirectoryTreeViewModel is not null)
            {
                return NewOpenedTreeViewModelHistory(
                    inInputFileState,
                    parentDirectoryTreeViewModel,
                    openParentDirectoryAction.LuthetusIdeComponentRenderers,
                    openParentDirectoryAction.LuthetusCommonComponentRenderers,
                    openParentDirectoryAction.FileSystemProvider,
                    openParentDirectoryAction.EnvironmentProvider);
            }

            return inInputFileState;
        }

        [ReducerMethod]
        public static InputFileRegistry ReduceRefreshCurrentSelectionAction(
            InputFileRegistry inInputFileState,
            RefreshCurrentSelectionAction refreshCurrentSelectionAction)
        {
            var currentSelection = inInputFileState
                .OpenedTreeViewModelHistory[inInputFileState.IndexInHistory];

            currentSelection.Children.Clear();

            refreshCurrentSelectionAction.BackgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), CommonBackgroundTaskWorker.Queue.Key,
                "Refresh Current Selection",
                async () =>
                {
                    await currentSelection.LoadChildrenAsync();
                });

            return inInputFileState;
        }

        [ReducerMethod]
        public static InputFileRegistry ReduceSetSearchQueryAction(
            InputFileRegistry inInputFileState,
            SetSearchQueryAction setSearchQueryAction)
        {
            var openedTreeViewModel = inInputFileState
                .OpenedTreeViewModelHistory[
                    inInputFileState.IndexInHistory];

            foreach (var treeViewModel in openedTreeViewModel.Children)
            {
                var treeViewAbsolutePath = (TreeViewAbsolutePath)treeViewModel;

                treeViewModel.IsHidden = !treeViewAbsolutePath.Item.NameWithExtension
                    .Contains(
                        setSearchQueryAction.SearchQuery,
                        StringComparison.InvariantCultureIgnoreCase);
            }

            return inInputFileState with
            {
                SearchQuery = setSearchQueryAction.SearchQuery
            };
        }
    }
}
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.TreeViewImplementations;

namespace Luthetus.Ide.ClassLib.Store.InputFileCase;

public partial record InputFileState
{
    private static class Reducer
    {
        [ReducerMethod]
        public static InputFileState ReduceStartInputFileStateFormAction(
            InputFileState inInputFileState,
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
        public static InputFileState ReduceSetSelectedTreeViewModelAction(
            InputFileState inInputFileState,
            SetSelectedTreeViewModelAction setSelectedTreeViewModelAction)
        {
            return inInputFileState with
            {
                SelectedTreeViewModel =
                    setSelectedTreeViewModelAction.SelectedTreeViewModel
            };
        }

        [ReducerMethod]
        public static InputFileState ReduceSetOpenedTreeViewModelAction(
            InputFileState inInputFileState,
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
        public static InputFileState ReduceSetSelectedInputFilePatternAction(
            InputFileState inInputFileState,
            SetSelectedInputFilePatternAction setSelectedInputFilePatternAction)
        {
            return inInputFileState with
            {
                SelectedInputFilePattern =
                    setSelectedInputFilePatternAction.InputFilePattern
            };
        }

        [ReducerMethod]
        public static InputFileState ReduceMoveBackwardsInHistoryAction(
            InputFileState inInputFileState,
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
        public static InputFileState ReduceMoveForwardsInHistoryAction(
            InputFileState inInputFileState,
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
        public static InputFileState ReduceOpenParentDirectoryAction(
            InputFileState inInputFileState,
            OpenParentDirectoryAction openParentDirectoryAction)
        {
            var currentSelection = inInputFileState
                .OpenedTreeViewModelHistory[inInputFileState.IndexInHistory];

            TreeViewAbsoluteFilePath? parentDirectoryTreeViewModel = null;

            // If has a ParentDirectory select it
            if (currentSelection.Item.AncestorDirectories.Any())
            {
                var parentDirectoryAbsoluteFilePath =
                    currentSelection.Item.AncestorDirectories.Last();

                parentDirectoryTreeViewModel = new TreeViewAbsoluteFilePath(
                    (IAbsoluteFilePath)parentDirectoryAbsoluteFilePath,
                    openParentDirectoryAction.LuthetusIdeComponentRenderers,
                    openParentDirectoryAction.LuthetusCommonComponentRenderers,
                    openParentDirectoryAction.FileSystemProvider,
                    openParentDirectoryAction.EnvironmentProvider,
                    false,
                    true);

                var backgroundTask = new BackgroundTask(
                    async cancellationToken =>
                    {
                        await parentDirectoryTreeViewModel.LoadChildrenAsync();
                    },
                    "ReduceOpenParentDirectoryActionTask",
                    "TODO: Describe this task",
                    false,
                    _ => Task.CompletedTask,
                    null,
                    CancellationToken.None);

                openParentDirectoryAction.LuthetusCommonBackgroundTaskService
                    .QueueBackgroundWorkItem(backgroundTask);
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
        public static InputFileState ReduceRefreshCurrentSelectionAction(
            InputFileState inInputFileState,
            RefreshCurrentSelectionAction refreshCurrentSelectionAction)
        {
            var currentSelection = inInputFileState
                .OpenedTreeViewModelHistory[inInputFileState.IndexInHistory];

            currentSelection.Children.Clear();

            var backgroundTask = new BackgroundTask(
                async cancellationToken =>
                {
                    await currentSelection.LoadChildrenAsync();
                },
                "ReduceRefreshCurrentSelectionActionTask",
                "TODO: Describe this task",
                false,
                _ => Task.CompletedTask,
                null,
                CancellationToken.None);

            refreshCurrentSelectionAction.LuthetusCommonBackgroundTaskService
                .QueueBackgroundWorkItem(backgroundTask);

            return inInputFileState;
        }

        [ReducerMethod]
        public static InputFileState ReduceSetSearchQueryAction(
            InputFileState inInputFileState,
            SetSearchQueryAction setSearchQueryAction)
        {
            var openedTreeViewModel = inInputFileState
                .OpenedTreeViewModelHistory[
                    inInputFileState.IndexInHistory];

            foreach (var treeViewModel in openedTreeViewModel.Children)
            {
                var treeViewAbsoluteFilePath = (TreeViewAbsoluteFilePath)treeViewModel;

                treeViewModel.IsHidden = !treeViewAbsoluteFilePath.Item.FilenameWithExtension
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
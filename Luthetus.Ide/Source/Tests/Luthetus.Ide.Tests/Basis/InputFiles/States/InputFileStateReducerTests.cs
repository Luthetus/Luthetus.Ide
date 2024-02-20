using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.Tests.Basis.InputFiles.States;

public class InputFileStateReducerTests
{
    private static class Reducer
    {
        [Fact]
        public void ReduceStartInputFileStateFormAction()
        {
            //[ReducerMethod]
            //public static InputFileState (
            //    InputFileState inState,
            //    StartInputFileStateFormAction startInputFileStateFormAction)
        }

        [Fact]
        public void ReduceSetSelectedTreeViewModelAction()
        {
            //[ReducerMethod]
            //public static InputFileState (
            //    InputFileState inState,
            //    SetSelectedTreeViewModelAction setSelectedTreeViewModelAction)
        }

        [Fact]
        public void ReduceSetOpenedTreeViewModelAction()
        {
            //[ReducerMethod]
            //public static InputFileState (
            //    InputFileState inState,
            //    SetOpenedTreeViewModelAction setOpenedTreeViewModelAction)
        }

        [Fact]
        public void ReduceSetSelectedInputFilePatternAction()
        {
            //[ReducerMethod]
            //public static InputFileState (
            //    InputFileState inState,
            //    SetSelectedInputFilePatternAction setSelectedInputFilePatternAction)
        }


        [Fact]
        public void ReduceMoveBackwardsInHistoryAction()
        {
            //[ReducerMethod(typeof(MoveBackwardsInHistoryAction))]
            //public static InputFileState (InputFileState inState)
        }

        [Fact]
        public void ReduceMoveForwardsInHistoryAction()
        {
            //[ReducerMethod(typeof(MoveForwardsInHistoryAction))]
            //public static InputFileState (InputFileState inState)
        }

        [Fact]
        public void ReduceOpenParentDirectoryAction()
        {
            //[ReducerMethod]
            //public static InputFileState (
            //    InputFileState inState,
            //    OpenParentDirectoryAction openParentDirectoryAction)
        }

        [Fact]
        public void ReduceRefreshCurrentSelectionAction()
        {
            //[ReducerMethod]
            //public static InputFileState (
            //    InputFileState inState,
            //    RefreshCurrentSelectionAction refreshCurrentSelectionAction)
        }

        [Fact]
        public void ReduceSetSearchQueryAction()
        {
            //[ReducerMethod]
            //public static InputFileState (
            //    InputFileState inState,
            //    SetSearchQueryAction setSearchQueryAction)
        }
    }
}
using Fluxor;
using Luthetus.TextEditor.RazorLib.Diffs.Models;

namespace Luthetus.TextEditor.RazorLib.Diffs.States;

public partial class TextEditorDiffState
{
    public class Reducer
    {
        [ReducerMethod]
        public static TextEditorDiffState ReduceDisposeAction(
            TextEditorDiffState inState,
            DisposeAction disposeAction)
        {
            var inDiff = inState.DiffModelList.FirstOrDefault(
                x => x.DiffKey == disposeAction.DiffKey);

            if (inDiff is null)
                return inState;

            var outDiffModelList = inState.DiffModelList.Remove(inDiff);

            return new TextEditorDiffState
            {
                DiffModelList = outDiffModelList
            };
        }

        [ReducerMethod]
        public static TextEditorDiffState ReduceRegisterAction(
            TextEditorDiffState inState,
            RegisterAction registerAction)
        {
            var inDiff = inState.DiffModelList.FirstOrDefault(
                x => x.DiffKey == registerAction.DiffKey);

            if (inDiff is not null)
                return inState;

            var diff = new TextEditorDiffModel(
                registerAction.DiffKey,
                registerAction.InViewModelKey,
                registerAction.OutViewModelKey);

            var outDiffModelList = inState.DiffModelList.Add(diff);

            return new TextEditorDiffState
            {
                DiffModelList = outDiffModelList
            };
        }
    }
}
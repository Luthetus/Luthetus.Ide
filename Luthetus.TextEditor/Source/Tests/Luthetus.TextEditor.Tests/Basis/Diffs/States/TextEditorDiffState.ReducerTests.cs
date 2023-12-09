using Fluxor;
using Luthetus.TextEditor.RazorLib.Diffs.Models;

namespace Luthetus.TextEditor.RazorLib.Diffs.States;

public partial class TextEditorDiffState
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorDiffState ReduceDisposeAction(
            TextEditorDiffState inState,
            DisposeAction disposeAction)
        {
            var inDiff = inState.DiffModelBag.FirstOrDefault(
                x => x.DiffKey == disposeAction.DiffKey);

            if (inDiff is null)
                return inState;

            var outDiffModelBag = inState.DiffModelBag.Remove(inDiff);

            return new TextEditorDiffState
            {
                DiffModelBag = outDiffModelBag
            };
        }

        [ReducerMethod]
        public static TextEditorDiffState ReduceRegisterAction(
            TextEditorDiffState inState,
            RegisterAction registerAction)
        {
            var inDiff = inState.DiffModelBag.FirstOrDefault(
                x => x.DiffKey == registerAction.DiffKey);

            if (inDiff is not null)
                return inState;

            var diff = new TextEditorDiffModel(
                registerAction.DiffKey,
                registerAction.BeforeViewModelKey,
                registerAction.AfterViewModelKey);

            var outDiffModelBag = inState.DiffModelBag.Add(diff);

            return new TextEditorDiffState
            {
                DiffModelBag = outDiffModelBag
            };
        }
    }
}
using Fluxor;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

public partial class TextEditorViewModelStateTests
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorViewModelState ReduceRegisterAction(
            TextEditorViewModelState inState,
            RegisterAction registerAction)
        {
            var inViewModel = inState.ViewModelBag.FirstOrDefault(
                x => x.ViewModelKey == registerAction.ViewModelKey);

            if (inViewModel is not null)
                return inState;

            var viewModel = new TextEditorViewModel(
                registerAction.ViewModelKey,
                registerAction.ResourceUri,
                registerAction.TextEditorService,
                VirtualizationResult<List<RichCharacter>>.GetEmptyRichCharacters(),
                false);

            var outViewModelBag = inState.ViewModelBag.Add(viewModel);

            return new TextEditorViewModelState { ViewModelBag = outViewModelBag };
        }

        [ReducerMethod]
        public static TextEditorViewModelState ReduceDisposeAction(
            TextEditorViewModelState inState,
            DisposeAction disposeAction)
        {
            var inViewModel = inState.ViewModelBag.FirstOrDefault(
                x => x.ViewModelKey == disposeAction.ViewModelKey);

            if (inViewModel is null)
                return inState;

            var outViewModelBag = inState.ViewModelBag.Remove(inViewModel);
            inViewModel.Dispose();

            return new TextEditorViewModelState { ViewModelBag = outViewModelBag };
        }

        [ReducerMethod]
        public static TextEditorViewModelState ReduceSetViewModelWithAction(
            TextEditorViewModelState inState,
            SetViewModelWithAction setViewModelWithAction)
        {
            var inViewModel = inState.ViewModelBag.FirstOrDefault(
                x => x.ViewModelKey == setViewModelWithAction.ViewModelKey);

            if (inViewModel is null)
                return inState;

            var outViewModel = setViewModelWithAction.WithFunc.Invoke(inViewModel);

            var outViewModelBag = inState.ViewModelBag.Replace(inViewModel, outViewModel);

            return new TextEditorViewModelState { ViewModelBag = outViewModelBag };
        }
    }
}
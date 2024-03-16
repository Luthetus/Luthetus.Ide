using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

public partial class TextEditorViewModelState
{
    public class Reducer
    {
        [ReducerMethod]
        public static TextEditorViewModelState ReduceRegisterAction(
            TextEditorViewModelState inState,
            RegisterAction registerAction)
        {
            var inViewModel = inState.ViewModelList.FirstOrDefault(
                x => x.ViewModelKey == registerAction.ViewModelKey);

            if (inViewModel is not null)
                return inState;

            if (registerAction.ViewModelKey == Key<TextEditorViewModel>.Empty)
                throw new InvalidOperationException($"Provided {nameof(Key<TextEditorViewModel>)} cannot be {nameof(Key<TextEditorViewModel>)}.{Key<TextEditorViewModel>.Empty}");

            var viewModel = new TextEditorViewModel(
                registerAction.ViewModelKey,
                registerAction.ResourceUri,
                registerAction.TextEditorService,
                VirtualizationResult<List<RichCharacter>>.GetEmptyRichCharacters(),
                false,
                registerAction.Category);

            var outViewModelList = inState.ViewModelList.Add(viewModel);

            return new TextEditorViewModelState
			{
				ViewModelList = outViewModelList,
			};
        }

        [ReducerMethod]
        public static TextEditorViewModelState ReduceDisposeAction(
            TextEditorViewModelState inState,
            DisposeAction disposeAction)
        {
            var inViewModel = inState.ViewModelList.FirstOrDefault(
                x => x.ViewModelKey == disposeAction.ViewModelKey);

            if (inViewModel is null)
                return inState;

            var outViewModelList = inState.ViewModelList.Remove(inViewModel);
			
            inViewModel.Dispose();

            return new TextEditorViewModelState
			{
				ViewModelList = outViewModelList,
			};
        }

        [ReducerMethod]
        public static TextEditorViewModelState ReduceSetViewModelWithAction(
            TextEditorViewModelState inState,
            SetViewModelWithAction setViewModelWithAction)
        {
            var inViewModel = inState.ViewModelList.FirstOrDefault(
                x => x.ViewModelKey == setViewModelWithAction.ViewModelKey);

            if (inViewModel is null)
                return inState;

            var outViewModel = setViewModelWithAction.WithFunc.Invoke(inViewModel);

            var outViewModelList = inState.ViewModelList.Replace(inViewModel, outViewModel);

            return new TextEditorViewModelState
			{
				ViewModelList = outViewModelList,
			};
        }
    }
}
using Fluxor;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

public partial class TextEditorModelState
{
    public class Reducer
    {
        [ReducerMethod]
        public static TextEditorModelState ReduceRegisterAction(
            TextEditorModelState inState,
            RegisterAction registerAction)
        {
            if (inState.ModelBag.Any(x => x.ResourceUri == registerAction.Model.ResourceUri))
                return inState;

            var outModelBag = inState.ModelBag.Add(registerAction.Model);

            return new TextEditorModelState { ModelBag = outModelBag };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceDisposeAction(
            TextEditorModelState inState,
            DisposeAction disposeAction)
        {
            var inModel = inState.ModelBag.SingleOrDefault(
                x => x.ResourceUri == disposeAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModelBag = inState.ModelBag.Remove(inModel);

            return new TextEditorModelState { ModelBag = outModelBag };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceSetModelAction(
            TextEditorModelState inState,
            SetAction setModelAction)
        {
            var inModel = inState.ModelBag.FirstOrDefault(
                x => x.ResourceUri == setModelAction.ModelModifier.ResourceUri);

            if (inModel is null)
                return inState;

            var outViewModelBag = inState.ModelBag.Replace(
                inModel,
                setModelAction.ModelModifier.ToModel());

            return new TextEditorModelState { ModelBag = outViewModelBag };
        }
    }
}
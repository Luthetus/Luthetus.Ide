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
            if (inState.ModelList.Any(x => x.ResourceUri == registerAction.Model.ResourceUri))
                return inState;

            var outModelList = inState.ModelList.Add(registerAction.Model);

            return new TextEditorModelState { ModelList = outModelList };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceDisposeAction(
            TextEditorModelState inState,
            DisposeAction disposeAction)
        {
            var inModel = inState.ModelList.SingleOrDefault(
                x => x.ResourceUri == disposeAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModelList = inState.ModelList.Remove(inModel);

            return new TextEditorModelState { ModelList = outModelList };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceSetAction(
            TextEditorModelState inState,
            SetAction setAction)
        {
            var inModel = inState.ModelList.FirstOrDefault(
                x => x.ResourceUri == setAction.ModelModifier.ResourceUri);

            if (inModel is null)
                return inState;

            var outViewModelList = inState.ModelList.Replace(
                inModel,
                setAction.ModelModifier.ToModel());

            return new TextEditorModelState { ModelList = outViewModelList };
        }
    }
}
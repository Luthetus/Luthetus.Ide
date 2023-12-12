using Fluxor;
using System.Collections.Immutable;

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
            if (inState.ModelBag.Any(x =>
                    x.ResourceUri == registerAction.Model.ResourceUri ||
                    x.ResourceUri == registerAction.Model.ResourceUri))
            {
                return inState;
            }

            var outModelBag = inState.ModelBag.Add(registerAction.Model);

            return new TextEditorModelState { ModelBag = outModelBag };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceForceRerenderAction(
            TextEditorModelState inState,
            ForceRerenderAction forceRerenderAction)
        {
            var inModel = inState.ModelBag.SingleOrDefault(x => x.ResourceUri == forceRerenderAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModel = inModel.PerformForceRerenderAction(forceRerenderAction);
            var outModelBag = inState.ModelBag.Replace(inModel, outModel);

            return new TextEditorModelState { ModelBag = outModelBag };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceInsertTextAction(
            TextEditorModelState inState,
            InsertTextAction insertTextAction)
        {
            var inModel = inState.ModelBag.SingleOrDefault(x => x.ResourceUri == insertTextAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModel = inModel.PerformEditTextEditorAction(insertTextAction);
            var outModelBag = inState.ModelBag.Replace(inModel, outModel);

            return new TextEditorModelState { ModelBag = outModelBag };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceKeyboardEventAction(
            TextEditorModelState inState,
            KeyboardEventAction keyboardEventAction)
        {
            var inModel = inState.ModelBag.SingleOrDefault(x => x.ResourceUri == keyboardEventAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModel = inModel.PerformEditTextEditorAction(keyboardEventAction);
            var outModelBag = inState.ModelBag.Replace(inModel, outModel);

            return new TextEditorModelState { ModelBag = outModelBag };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceDeleteTextByMotionAction(
            TextEditorModelState inState,
            DeleteTextByMotionAction deleteTextByMotionAction)
        {
            var inModel = inState.ModelBag.SingleOrDefault(x => x.ResourceUri == deleteTextByMotionAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModel = inModel.PerformEditTextEditorAction(deleteTextByMotionAction);
            var outModelBag = inState.ModelBag.Replace(inModel, outModel);

            return new TextEditorModelState { ModelBag = outModelBag };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceDeleteTextByRangeAction(
            TextEditorModelState inState,
            DeleteTextByRangeAction deleteTextByRangeAction)
        {
            var inModel = inState.ModelBag.SingleOrDefault(x => x.ResourceUri == deleteTextByRangeAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModel = inModel.PerformEditTextEditorAction(deleteTextByRangeAction);
            var outModelBag = inState.ModelBag.Replace(inModel, outModel);

            return new TextEditorModelState { ModelBag = outModelBag };
        }
        
        [ReducerMethod]
        public static TextEditorModelState ReduceRegisterPresentationModelAction(
            TextEditorModelState inState,
            RegisterPresentationModelAction registerPresentationModelAction)
        {
            var inModel = inState.ModelBag.SingleOrDefault(x => x.ResourceUri == registerPresentationModelAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModel = inModel.PerformRegisterPresentationModelAction(registerPresentationModelAction);
            var outModelBag = inState.ModelBag.Replace(inModel, outModel);

            return new TextEditorModelState { ModelBag = outModelBag };
        }
        
        [ReducerMethod]
        public static TextEditorModelState ReduceCalculatePresentationModelAction(
            TextEditorModelState inState,
            CalculatePresentationModelAction calculatePresentationModelAction)
        {
            var inModel = inState.ModelBag.SingleOrDefault(x => x.ResourceUri == calculatePresentationModelAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModel = inModel.PerformCalculatePresentationModelAction(calculatePresentationModelAction);
            var outModelBag = inState.ModelBag.Replace(inModel, outModel);

            return new TextEditorModelState { ModelBag = outModelBag };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceUndoEditAction(
            TextEditorModelState inState,
            UndoEditAction undoEditAction)
        {
            var inModel = inState.ModelBag.SingleOrDefault(x => x.ResourceUri == undoEditAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModel = inModel.UndoEdit();
            var outModelBag = inState.ModelBag.Replace(inModel, outModel);

            return new TextEditorModelState { ModelBag = outModelBag };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceRedoEditAction(
            TextEditorModelState inState,
            RedoEditAction redoEditAction)
        {
            var inModel = inState.ModelBag.SingleOrDefault(x => x.ResourceUri == redoEditAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModel = inModel.RedoEdit();
            var outModelBag = inState.ModelBag.Replace(inModel, outModel);

            return new TextEditorModelState { ModelBag = outModelBag };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceReloadAction(
            TextEditorModelState inState,
            ReloadAction reloadAction)
        {
            var inModel = inState.ModelBag.SingleOrDefault(x => x.ResourceUri == reloadAction.ResourceUri);

            if (inModel is null)
                return inState;

            var modelModifier = new TextEditorModelModifier(inModel);

            modelModifier.ModifyContent(reloadAction.Content);
			modelModifier.ModifyResourceData(reloadAction.ResourceUri, reloadAction.ResourceLastWriteTime);

            var outModelBag = inState.ModelBag.Replace(inModel, modelModifier.ToTextEditorModel());

            return new TextEditorModelState { ModelBag = outModelBag };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceSetResourceDataAction(
            TextEditorModelState inState,
            SetResourceDataAction setResourceDataAction)
        {
            var inModel = inState.ModelBag.SingleOrDefault(x => x.ResourceUri == setResourceDataAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModel = inModel.SetResourceData(setResourceDataAction.ResourceUri, setResourceDataAction.ResourceLastWriteTime);
            var outModelBag = inState.ModelBag.Replace(inModel, outModel);

            return new TextEditorModelState { ModelBag = outModelBag };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceDisposeAction(
            TextEditorModelState inState,
            DisposeAction disposeAction)
        {
            var inModel = inState.ModelBag.SingleOrDefault(x => x.ResourceUri == disposeAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModelBag = inState.ModelBag.Remove(inModel);

            return new TextEditorModelState { ModelBag = outModelBag };
        }

        [ReducerMethod]
        public static TextEditorModelState ReduceSetUsingRowEndingKindAction(
            TextEditorModelState inState,
            SetUsingRowEndingKindAction setUsingRowEndingKindAction)
        {
            var inModel = inState.ModelBag.SingleOrDefault(x => x.ResourceUri == setUsingRowEndingKindAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModel = inModel.SetUsingRowEndingKind(setUsingRowEndingKindAction.RowEndingKind);
            var outModelBag = inState.ModelBag.Replace(inModel, outModel);

            return new TextEditorModelState { ModelBag = outModelBag };
        }
    }
}
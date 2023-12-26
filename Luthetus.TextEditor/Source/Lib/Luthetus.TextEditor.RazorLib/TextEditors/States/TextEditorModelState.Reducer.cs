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
            // TODO: Is the Any() predicate redundant?...
            // ...I'm half asleep and don't want to remove the other part of the || without...
            // ...being sure. I think they're the same expressions or'd with eachother
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
        public static TextEditorModelState ReduceSetModelAction(
            TextEditorModelState inState,
            SetModelAction setModelAction)
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
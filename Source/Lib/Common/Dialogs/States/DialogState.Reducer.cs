using Fluxor;

namespace Luthetus.Common.RazorLib.Dialogs.States;

public partial record DialogState
{
    public class Reducer
    {
        [ReducerMethod]
        public static DialogState ReduceRegisterAction(
            DialogState inState,
            RegisterAction registerAction)
        {
            if (inState.DialogList.Any(x => x.DynamicViewModelKey == registerAction.Dialog.DynamicViewModelKey))
                return inState;

            var outDialogList = inState.DialogList.Add(registerAction.Dialog);

            return inState with 
            {
                DialogList = outDialogList,
                ActiveDialogKey = registerAction.Dialog.DynamicViewModelKey,
            };
        }

        [ReducerMethod]
        public static DialogState ReduceSetIsMaximizedAction(
            DialogState inState,
            SetIsMaximizedAction setIsMaximizedAction)
        {
            var inDialog = inState.DialogList.FirstOrDefault(
                x => x.DynamicViewModelKey == setIsMaximizedAction.DynamicViewModelKey);

            if (inDialog is null)
                return inState;

            var outDialogList = inState.DialogList.Replace(
				inDialog,
				inDialog.SetDialogIsMaximized(setIsMaximizedAction.IsMaximized));

            return inState with { DialogList = outDialogList };
        }
        
        [ReducerMethod]
        public static DialogState ReduceSetActiveDialogKeyAction(
            DialogState inState,
            SetActiveDialogKeyAction setActiveDialogKeyAction)
        {
            return inState with { ActiveDialogKey = setActiveDialogKeyAction.DynamicViewModelKey };
        }

        [ReducerMethod]
        public static DialogState ReduceDisposeAction(
            DialogState inState,
            DisposeAction disposeAction)
        {
            var inDialog = inState.DialogList.FirstOrDefault(
                x => x.DynamicViewModelKey == disposeAction.DynamicViewModelKey);

            if (inDialog is null)
                return inState;

            var outDialogList = inState.DialogList.Remove(inDialog);

            return inState with { DialogList = outDialogList };
        }
    }
}
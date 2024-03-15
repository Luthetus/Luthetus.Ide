using Fluxor;
using Luthetus.Common.RazorLib.PolymorphicUis.Models;

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
            if (inState.DialogList.Any(x => x.DialogKey == registerAction.Dialog.DialogKey))
                return inState;

            var outDialogList = inState.DialogList.Add(registerAction.Dialog);

			registerAction.Dialog.IsDialog = true;

            return inState with 
            {
                DialogList = outDialogList,
                ActiveDialogKey = registerAction.Dialog.DialogKey,
            };
        }

        [ReducerMethod]
        public static DialogState ReduceSetIsMaximizedAction(
            DialogState inState,
            SetIsMaximizedAction setIsMaximizedAction)
        {
            var inDialog = inState.DialogList.FirstOrDefault(
                x => x.DialogKey == setIsMaximizedAction.DialogKey);

            if (inDialog is null)
                return inState;

            var outDialogList = inState.DialogList.Replace(
				inDialog,
				inDialog.DialogSetIsMaximized(setIsMaximizedAction.IsMaximized));

            return inState with { DialogList = outDialogList };
        }
        
        [ReducerMethod]
        public static DialogState ReduceSetActiveDialogKeyAction(
            DialogState inState,
            SetActiveDialogKeyAction setActiveDialogKeyAction)
        {
            return inState with { ActiveDialogKey = setActiveDialogKeyAction.DialogKey };
        }

        [ReducerMethod]
        public static DialogState ReduceDisposeAction(
            DialogState inState,
            DisposeAction disposeAction)
        {
            var inDialog = inState.DialogList.FirstOrDefault(
                x => x.DialogKey == disposeAction.DialogKey);

            if (inDialog is null)
                return inState;

            var outDialogList = inState.DialogList.Remove(inDialog);

			inDialog.IsDialog = false;

            return inState with { DialogList = outDialogList };
        }
    }
}
using Fluxor;
using Luthetus.Common.RazorLib.Dynamics.Models;

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
            {
            	registerAction.WasAlreadyRegistered = true;
            	return inState;
            }

			var outDialogList = new List<IDialog>(inState.DialogList);
            outDialogList.Add(registerAction.Dialog);

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
            var indexDialog = inState.DialogList.FindIndex(
                x => x.DynamicViewModelKey == setIsMaximizedAction.DynamicViewModelKey);

            if (indexDialog == -1)
                return inState;
                
            var inDialog = inState.DialogList[indexDialog];

            var outDialogList = new List<IDialog>(inState.DialogList);
            
            outDialogList[indexDialog] = inDialog.SetDialogIsMaximized(setIsMaximizedAction.IsMaximized);

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
            var indexDialog = inState.DialogList.FindIndex(
                x => x.DynamicViewModelKey == disposeAction.DynamicViewModelKey);

            if (indexDialog == -1)
                return inState;

			var inDialog = inState.DialogList[indexDialog];

            var outDialogList = new List<IDialog>(inState.DialogList);
            outDialogList.RemoveAt(indexDialog);

            return inState with { DialogList = outDialogList };
        }
    }
}
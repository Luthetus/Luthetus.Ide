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
            if (inState.DialogBag.Any(x => x.Key == registerAction.Entry.Key))
                return inState;

            var outDialogBag = inState.DialogBag.Add(registerAction.Entry);

            return new DialogState { DialogBag = outDialogBag };
        }

        [ReducerMethod]
        public static DialogState ReduceSetIsMaximizedAction(
            DialogState inState,
            SetIsMaximizedAction setIsMaximizedAction)
        {
            var inDialog = inState.DialogBag.FirstOrDefault(
                x => x.Key == setIsMaximizedAction.Key);

            if (inDialog is null)
                return inState;

            var outDialogBag = inState.DialogBag.Replace(inDialog, inDialog with
            {
                IsMaximized = setIsMaximizedAction.IsMaximized
            });

            return new DialogState { DialogBag = outDialogBag };
        }

        [ReducerMethod]
        public static DialogState ReduceDisposeAction(
            DialogState inState,
            DisposeAction disposeAction)
        {
            var inDialog = inState.DialogBag.FirstOrDefault(
                x => x.Key == disposeAction.Key);

            if (inDialog is null)
                return inState;

            var outDialogBag = inState.DialogBag.Remove(inDialog);

            return new DialogState
            {
                DialogBag = outDialogBag
            };
        }
    }
}
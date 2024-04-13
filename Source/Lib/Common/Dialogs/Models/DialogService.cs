using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Models;

public class DialogService : IDialogService
{
    private readonly IDispatcher _dispatcher;

    public DialogService(
        IDispatcher dispatcher,
        IState<DialogState> dialogStateWrap)
    {
        _dispatcher = dispatcher;
        DialogStateWrap = dialogStateWrap;
    }

    public IState<DialogState> DialogStateWrap { get; }

    public void RegisterDialogRecord(IDialog dialogRecord)
    {
        _dispatcher.Dispatch(new DialogState.RegisterAction(
            dialogRecord));
    }

    public void SetDialogRecordIsMaximized(Key<IDynamicViewModel> dynamicViewModelKey, bool isMaximized)
    {
        _dispatcher.Dispatch(new DialogState.SetIsMaximizedAction(
            dynamicViewModelKey,
            isMaximized));
    }

    public void DisposeDialogRecord(Key<IDynamicViewModel> dynamicViewModelKey)
    {
        _dispatcher.Dispatch(new DialogState.DisposeAction(
            dynamicViewModelKey));
    }
}
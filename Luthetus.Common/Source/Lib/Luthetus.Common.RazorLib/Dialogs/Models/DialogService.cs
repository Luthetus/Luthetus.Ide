using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Models;

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

    public void RegisterDialogRecord(IPolymorphicDialog dialogRecord)
    {
        _dispatcher.Dispatch(new DialogState.RegisterAction(
            dialogRecord));
    }

    public void SetDialogRecordIsMaximized(Key<IPolymorphicUiRecord> dialogKey, bool isMaximized)
    {
        _dispatcher.Dispatch(new DialogState.SetIsMaximizedAction(
            dialogKey,
            isMaximized));
    }

    public void DisposeDialogRecord(Key<IPolymorphicUiRecord> dialogKey)
    {
        _dispatcher.Dispatch(new DialogState.DisposeAction(
            dialogKey));
    }
}
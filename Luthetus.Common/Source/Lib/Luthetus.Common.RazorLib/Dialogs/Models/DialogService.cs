using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Models;

public class DialogService : IDialogService
{
    private readonly IDispatcher _dispatcher;

    public DialogService(
        bool isEnabled,
        IDispatcher dispatcher,
        IState<DialogState> dialogStateWrap)
    {
        _dispatcher = dispatcher;
        IsEnabled = isEnabled;
        DialogStateWrap = dialogStateWrap;
    }

    public bool IsEnabled { get; }
    public IState<DialogState> DialogStateWrap { get; }

    public void RegisterDialogRecord(DialogRecord dialogRecord)
    {
        _dispatcher.Dispatch(new DialogState.RegisterAction(
            dialogRecord));
    }

    public void SetDialogRecordIsMaximized(Key<DialogRecord> dialogKey, bool isMaximized)
    {
        _dispatcher.Dispatch(new DialogState.SetIsMaximizedAction(
            dialogKey,
            isMaximized));
    }

    public void DisposeDialogRecord(Key<DialogRecord> dialogKey)
    {
        _dispatcher.Dispatch(new DialogState.DisposeAction(
            dialogKey));
    }
}
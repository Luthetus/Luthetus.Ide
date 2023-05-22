using BlazorCommon.RazorLib.Dialog;
using BlazorCommon.RazorLib.Store.DialogCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Settings;

public partial class SettingsDialogEntryPoint : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private DialogRecord _dialogRecord = new(
        DialogKey.NewDialogKey(),
        "Settings",
        typeof(SettingsDisplay),
        null,
        null)
    {
        IsResizable = true
    };

    public void DispatchRegisterDialogRecordAction()
    {
        Dispatcher.Dispatch(
            new DialogRecordsCollection.RegisterAction(
                _dialogRecord));
    }
}
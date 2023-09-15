using Fluxor;
using Luthetus.Common.RazorLib.Dialog.Models;
using Luthetus.Common.RazorLib.Dialog.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.SettingsCase.Displays;

public partial class SettingsDialogEntryPoint : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private DialogRecord _dialogRecord = new(
        DialogKey.NewKey(),
        "Settings",
        typeof(SettingsDisplay),
        null,
        null)
    {
        IsResizable = true
    };

    public void DispatchRegisterDialogRecordAction()
    {
        Dispatcher.Dispatch(new DialogRegistry.RegisterAction(
            _dialogRecord));
    }
}
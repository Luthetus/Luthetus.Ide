using Fluxor;
using Luthetus.Common.RazorLib.Dialog.Models;
using Luthetus.Common.RazorLib.Dialog.States;
using Luthetus.Common.RazorLib.KeyCase.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.SettingsCase.Displays;

public partial class SettingsDialogEntryPoint : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private DialogRecord _dialogRecord = new(
        Key<DialogRecord>.NewKey(),
        "Settings",
        typeof(SettingsDisplay),
        null,
        null)
    {
        IsResizable = true
    };

    public void DispatchRegisterDialogRecordAction()
    {
        Dispatcher.Dispatch(new DialogState.RegisterAction(
            _dialogRecord));
    }
}
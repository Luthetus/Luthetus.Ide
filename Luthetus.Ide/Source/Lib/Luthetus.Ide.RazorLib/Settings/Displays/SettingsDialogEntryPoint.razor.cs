using Fluxor;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Settings.Displays;

public partial class SettingsDialogEntryPoint : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private DialogRecord _dialogRecord = new(
        Key<IPolymorphicUiRecord>.NewKey(),
        "Settings",
        typeof(SettingsDisplay),
        null,
        null,
		true);

    public void DispatchRegisterDialogRecordAction() =>
        Dispatcher.Dispatch(new DialogState.RegisterAction(_dialogRecord));
}
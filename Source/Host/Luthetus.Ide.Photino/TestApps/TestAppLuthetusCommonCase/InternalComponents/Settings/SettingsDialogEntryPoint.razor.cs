using Luthetus.Common.RazorLib.Dialog;
using Luthetus.Common.RazorLib.Store.DialogCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Luthetus.Ide.Photino.TestApps.TestAppLuthetusCommonCase.InternalComponents.RenderCounter;

namespace Luthetus.Ide.Photino.TestApps.TestAppLuthetusCommonCase.InternalComponents.Settings;

public partial class SettingsDialogEntryPoint : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private RenderCounterDisplay _renderCounterDisplayComponent = null!;

    private static readonly DialogRecord _dialogRecord = new(
        DialogKey.NewDialogKey(),
        "Settings",
        typeof(SettingsDisplay),
        null,
        null)
    {
        IsResizable = true
    };

    protected override void OnAfterRender(bool firstRender)
    {
        _renderCounterDisplayComponent.IncrementCount();

        base.OnAfterRender(firstRender);
    }

    public void DispatchRegisterDialogRecordAction()
    {
        Dispatcher.Dispatch(
            new DialogRecordsCollection.RegisterAction(
                _dialogRecord));
    }
}
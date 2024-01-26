using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

public partial class IdeImportExportButtons : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private DialogRecord _importDialogRecord = new(
        Key<DialogRecord>.NewKey(),
        "Import",
        typeof(IdeImportDisplay),
        null,
        null)
        {
            IsResizable = true
        };

    private DialogRecord _exportDialogRecord = new(
        Key<DialogRecord>.NewKey(),
        "Export",
        typeof(IdeExportDisplay),
        null,
        null)
        {
            IsResizable = true
        };

    private void ImportOnClick()
    {
        Dispatcher.Dispatch(new DialogState.RegisterAction(_importDialogRecord));
    }
    
    private void ExportOnClick()
    {
        Dispatcher.Dispatch(new DialogState.RegisterAction(_exportDialogRecord));
    }
}
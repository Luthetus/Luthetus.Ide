using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

public partial class IdeImportExportButtons : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private IDialog _importDialogRecord = new DialogViewModel(
        Key<IDynamicViewModel>.NewKey(),
        "Import",
        typeof(IdeImportDisplay),
        null,
        null,
		true,
		null);

    private IDialog _exportDialogRecord = new DialogViewModel(
        Key<IDynamicViewModel>.NewKey(),
        "Export",
        typeof(IdeExportDisplay),
        null,
        null,
		true,
		null);

    private void ImportOnClick()
    {
        Dispatcher.Dispatch(new DialogState.RegisterAction(_importDialogRecord));
    }
    
    private void ExportOnClick()
    {
        Dispatcher.Dispatch(new DialogState.RegisterAction(_exportDialogRecord));
    }
}
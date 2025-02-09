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
    [Inject]
    private IDialogService DialogService { get; set; } = null!;

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
        DialogService.ReduceRegisterAction(_importDialogRecord);
    }
    
    private void ExportOnClick()
    {
        DialogService.ReduceRegisterAction(_exportDialogRecord);
    }
}
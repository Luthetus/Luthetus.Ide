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

    private IDialogViewModel _importDialogRecord = new DialogViewModel(
        Key<IDialogViewModel>.NewKey(),
        "Import",
        typeof(IdeImportDisplay),
        null,
        null,
		true,
		null);

    private IDialogViewModel _exportDialogRecord = new DialogViewModel(
        Key<IDialogViewModel>.NewKey(),
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
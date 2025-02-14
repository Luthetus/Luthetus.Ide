using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

public partial class IdeImportExportButtons : ComponentBase
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

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
		CommonApi.DialogApi.ReduceRegisterAction(_importDialogRecord);
    }
    
    private void ExportOnClick()
    {
		CommonApi.DialogApi.ReduceRegisterAction(_exportDialogRecord);
    }
}
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Edits.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.Edits.Displays;

public partial class DirtyResourceUriInteractiveIconDisplay : FluxorComponent
{
    [Inject]
    private IState<DirtyResourceUriState> DirtyResourceUriStateWrap { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;

    //private static Key<DialogRecord> _dialogRecordKey = Key<DialogRecord>.NewKey();

    //private readonly DialogRecord _dialogRecord = new(
    //    _dialogRecordKey,
    //    "Un-saved Files",
    //    typeof(DirtyTextEditorModelsViewDisplay),
    //    null,
    //    null)
    //{
    //    IsResizable = true
    //};

    //private void ShowDialogOnClick()
    //{
    //    DialogService.RegisterDialogRecord(_dialogRecord);
    //}
}
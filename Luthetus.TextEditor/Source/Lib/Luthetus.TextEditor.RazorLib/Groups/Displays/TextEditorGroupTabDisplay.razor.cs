using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.Groups.Displays;

public partial class TextEditorGroupTabDisplay : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IState<TextEditorViewModelState> TextEditorViewModelStateWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorModelState> TextEditorModelStateWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<TextEditorViewModel> TextEditorViewModelKey { get; set; } = Key<TextEditorViewModel>.Empty;
    [Parameter, EditorRequired]
    public TextEditorGroup TextEditorGroup { get; set; } = null!;

    private string IsActiveCssClass => TextEditorGroup.ActiveViewModelKey == TextEditorViewModelKey
        ? "luth_active"
        : string.Empty;

    protected override void OnInitialized()
    {
        TextEditorViewModelStateWrap.StateChanged += TextEditorViewModelStateWrap_StateChanged;

        base.OnInitialized();
    }

    private void OnClickSetActiveTextEditorViewModel()
    {
        TextEditorService.GroupApi.SetActiveViewModel(TextEditorGroup.GroupKey, TextEditorViewModelKey);
    }

    private void OnMouseDown(MouseEventArgs mouseEventArgs)
    {
        if (mouseEventArgs.Button == 1)
            CloseTabOnClick();
    }

    private void CloseTabOnClick()
    {
        TextEditorService.GroupApi.RemoveViewModel(TextEditorGroup.GroupKey, TextEditorViewModelKey);
    }

    private async void TextEditorViewModelStateWrap_StateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        TextEditorViewModelStateWrap.StateChanged -= TextEditorViewModelStateWrap_StateChanged;
    }
}
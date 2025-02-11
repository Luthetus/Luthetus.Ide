using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class InputTextEditorCursorWidth : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    private const double MINIMUM_CURSOR_SIZE_IN_PIXELS = 1;

    private double TextEditorCursorWidth
    {
        get => TextEditorService.OptionsApi.GetTextEditorOptionsState().Options.CursorWidthInPixels;
        set
        {
            if (value < MINIMUM_CURSOR_SIZE_IN_PIXELS)
                value = MINIMUM_CURSOR_SIZE_IN_PIXELS;

            TextEditorService.OptionsApi.SetCursorWidth(value);
        }
    }
    
    protected override void OnInitialized()
    {
    	TextEditorService.OptionsApi.TextEditorOptionsStateChanged += TextEditorOptionsStateWrapOnStateChanged;
    	base.OnInitialized();
    }
    
    private async void TextEditorOptionsStateWrapOnStateChanged()
    {
        await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	TextEditorService.OptionsApi.TextEditorOptionsStateChanged -= TextEditorOptionsStateWrapOnStateChanged;
    }
}
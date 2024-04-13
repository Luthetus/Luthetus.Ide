using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class InputTextEditorFontSize : ComponentBase, IDisposable
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;

    public int FontSizeInPixels
    {
        get => TextEditorService.OptionsStateWrap.Value.Options.CommonOptions?.FontSizeInPixels ??
               TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS;
        set
        {
            if (value < TextEditorOptionsState.MINIMUM_FONT_SIZE_IN_PIXELS)
                value = TextEditorOptionsState.MINIMUM_FONT_SIZE_IN_PIXELS;

            _ = Task.Run(() => TextEditorService.OptionsApi.SetFontSize(value));
        }
    }

    protected override void OnInitialized()
    {
        TextEditorService.OptionsStateWrap.StateChanged += OptionsWrapOnStateChanged;

        base.OnInitialized();
    }

    private async void OptionsWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        TextEditorService.OptionsStateWrap.StateChanged -= OptionsWrapOnStateChanged;
    }
}
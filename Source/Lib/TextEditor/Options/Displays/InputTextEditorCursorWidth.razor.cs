using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class InputTextEditorCursorWidth : FluxorComponent
{
    [Inject]
    private IState<TextEditorOptionsState> TextEditorOptionsStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    public string TopLevelDivElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string InputElementCssClassString { get; set; } = string.Empty;
    [Parameter]
    public string LabelElementCssClassString { get; set; } = string.Empty;

    [CascadingParameter(Name = "InputElementCssClass")]
    public string CascadingInputElementCssClass { get; set; } = string.Empty;

    private const double MINIMUM_CURSOR_SIZE_IN_PIXELS = 1;

    private double TextEditorCursorWidth
    {
        get => TextEditorOptionsStateWrap.Value.Options.CursorWidthInPixels;
        set
        {
            if (value < MINIMUM_CURSOR_SIZE_IN_PIXELS)
                value = MINIMUM_CURSOR_SIZE_IN_PIXELS;

            TextEditorService.OptionsApi.SetCursorWidth(value);
        }
    }
}
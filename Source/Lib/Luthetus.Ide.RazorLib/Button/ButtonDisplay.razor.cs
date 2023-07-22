using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Button;

public partial class ButtonDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public Func<Task> OnClickFunc { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;
    [Parameter]
    public bool OnClickStopPropagation { get; set; }
    [Parameter]
    public bool IsDisabled { get; set; }
    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;

    private ElementReference? _buttonElementReference;

    public ElementReference? ButtonElementReference => _buttonElementReference;

    private string IsDisabledCssClass => IsDisabled
        ? "luth_disabled"
        : string.Empty;

    public async Task FireOnClickAction()
    {
        await OnClickFunc.Invoke();
    }
}
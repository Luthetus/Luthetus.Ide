using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.Options.Displays;

public partial class TextEditorSettings : ComponentBase
{
    [Parameter]
    public string InputElementCssClass { get; set; } = string.Empty;
}
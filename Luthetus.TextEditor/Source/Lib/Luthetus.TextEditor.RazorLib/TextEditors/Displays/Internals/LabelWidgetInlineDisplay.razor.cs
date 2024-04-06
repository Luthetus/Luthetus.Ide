using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class LabelWidgetInlineDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public string Text { get; set; } = null!;
}
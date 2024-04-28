using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class WidgetInlineLayerDisplay : ComponentBase
{
    [CascadingParameter]
    public RenderBatch RenderBatch { get; set; } = null!;
}
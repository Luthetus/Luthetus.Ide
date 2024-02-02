using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class FindOverlayDisplay : ComponentBase
{
    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;
}
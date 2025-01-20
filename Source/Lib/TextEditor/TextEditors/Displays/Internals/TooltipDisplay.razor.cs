using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TooltipDisplay : ComponentBase
{
	[Parameter, EditorRequired]
    public TextEditorRenderBatch? RenderBatch { get; set; }
}
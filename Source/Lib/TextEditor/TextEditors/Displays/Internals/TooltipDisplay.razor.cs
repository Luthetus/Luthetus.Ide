using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TooltipDisplay : ComponentBase
{
    [CascadingParameter]
    public TooltipViewModel? TextEditorTooltipViewModel { get; set; }
}
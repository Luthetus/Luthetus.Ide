using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.ContextCase;

public partial class ActiveContextEntryDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ContextRecord ContextRecord { get; set; } = null!;

    private bool _isExpanded;
}
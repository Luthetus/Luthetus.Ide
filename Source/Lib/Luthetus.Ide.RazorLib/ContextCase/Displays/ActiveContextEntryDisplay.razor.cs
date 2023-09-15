using Luthetus.Ide.RazorLib.ContextCase.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.ContextCase.Displays;

public partial class ActiveContextEntryDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ContextRecord ContextRecord { get; set; } = null!;

    private bool _isExpanded;
}
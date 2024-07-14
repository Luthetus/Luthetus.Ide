using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.OutOfBoundsClicks.Displays;

public partial class OutOfBoundsClickDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public int ZIndex { get; set; }
    [Parameter, EditorRequired]
    public Func<Task> OnMouseDownCallback { get; set; } = null!;
}
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Reactives.Displays;

public partial class ProgressDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public int MyProperty { get; set; }
}
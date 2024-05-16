using Luthetus.Ide.RazorLib.Gits.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitBranchCheckoutDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public GitState GitState { get; set; } = null!;
}
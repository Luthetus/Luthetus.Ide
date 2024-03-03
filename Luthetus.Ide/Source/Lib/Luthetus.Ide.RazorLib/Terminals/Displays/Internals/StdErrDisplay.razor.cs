using Luthetus.Ide.RazorLib.Terminals.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Terminals.Displays.Internals;

public partial class StdErrDisplay : ComponentBase
{
    [CascadingParameter, EditorRequired]
    public IntegratedTerminal IntegratedTerminal { get; set; } = null!;

    [Parameter, EditorRequired]
	public StdErr StdErr { get; set; } = null!;
}
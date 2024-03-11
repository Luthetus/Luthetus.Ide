using Luthetus.Ide.RazorLib.Terminals.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Terminals.Displays.Internals;

public partial class StdOutDisplay : ComponentBase
{
    [CascadingParameter, EditorRequired]
    public IntegratedTerminal IntegratedTerminal { get; set; } = null!;

    [Parameter, EditorRequired]
	public StdOut StdOut { get; set; } = null!;

	private string GetCssClass()
	{
		if (StdOut.StdOutKind == StdOutKind.Started || StdOut.StdOutKind == StdOutKind.Exited)
        	return "luth_te_keyword";
	    if (StdOut.StdOutKind == StdOutKind.Error)
	        return "luth_te_keyword-contextual";

		return string.Empty;
	}
}
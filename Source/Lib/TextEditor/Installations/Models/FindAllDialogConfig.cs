using Luthetus.TextEditor.RazorLib.FindAlls.Displays;

namespace Luthetus.TextEditor.RazorLib.Installations.Models;

public struct FindAllDialogConfig
{
	public FindAllDialogConfig()
	{
	}

    public Type ComponentRendererType { get; init; } = typeof(FindAllDisplay);
    public bool ComponentIsResizable { get; init; } = true;
}

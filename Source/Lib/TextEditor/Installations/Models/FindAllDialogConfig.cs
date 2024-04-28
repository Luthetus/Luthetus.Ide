using Luthetus.TextEditor.RazorLib.FindAlls.Displays;

namespace Luthetus.TextEditor.RazorLib.Installations.Models;

public class FindAllDialogConfig
{
    public Type ComponentRendererType { get; init; } = typeof(FindAllDisplay);
    public bool ComponentIsResizable { get; init; } = true;
}

using Luthetus.TextEditor.RazorLib.SearchEngines.Displays;

namespace Luthetus.TextEditor.RazorLib.Installations.Models;

public class FindAllDialogConfig
{
    public Type ComponentRendererType { get; init; } = typeof(TextEditorSearchEngineDisplay);
    public bool ComponentIsResizable { get; init; } = true;
}

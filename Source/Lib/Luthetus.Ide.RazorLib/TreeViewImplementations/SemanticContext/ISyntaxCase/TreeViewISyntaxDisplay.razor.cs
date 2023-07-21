namespace Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.ISyntaxCase;

public partial class TreeViewISyntaxDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ISyntax Syntax { get; set; } = null!;
}
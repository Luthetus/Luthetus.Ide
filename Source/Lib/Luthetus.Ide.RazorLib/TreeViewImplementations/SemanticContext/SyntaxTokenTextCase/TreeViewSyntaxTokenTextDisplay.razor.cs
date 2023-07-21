namespace Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.SyntaxTokenTextCase;

public partial class TreeViewSyntaxTokenTextDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ISyntaxToken SyntaxToken { get; set; } = null!;
}
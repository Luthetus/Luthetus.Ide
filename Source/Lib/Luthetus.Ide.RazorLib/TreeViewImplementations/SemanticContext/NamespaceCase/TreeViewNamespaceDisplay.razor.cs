namespace Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.NamespaceCase;

public partial class TreeViewNamespaceDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public BoundNamespaceStatementNode BoundNamespaceStatementNode { get; set; } = null!;
}
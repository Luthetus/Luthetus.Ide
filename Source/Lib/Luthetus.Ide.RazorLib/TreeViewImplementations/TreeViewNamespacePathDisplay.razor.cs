namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

public partial class TreeViewNamespacePathDisplay
    : ComponentBase, ITreeViewNamespacePathRendererType
{
    [Parameter, EditorRequired]
    public NamespacePath NamespacePath { get; set; } = null!;
}
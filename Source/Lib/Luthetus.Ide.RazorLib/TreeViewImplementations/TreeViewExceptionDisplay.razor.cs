namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

public partial class TreeViewExceptionDisplay
    : ComponentBase, ITreeViewExceptionRendererType
{
    [Parameter, EditorRequired]
    public TreeViewException TreeViewException { get; set; } = null!;
}
namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

public partial class TreeViewSolutionFolderDisplay
    : ComponentBase, ITreeViewSolutionFolderRendererType
{
    [Parameter, EditorRequired]
    public DotNetSolutionFolder DotNetSolutionFolder { get; set; } = null!;
}
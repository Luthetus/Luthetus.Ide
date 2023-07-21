namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

public partial class TreeViewCSharpProjectToProjectReferenceDisplay :
    ComponentBase, ITreeViewCSharpProjectToProjectReferenceRendererType
{
    [Parameter, EditorRequired]
    public CSharpProjectToProjectReference CSharpProjectToProjectReference { get; set; } = null!;
}
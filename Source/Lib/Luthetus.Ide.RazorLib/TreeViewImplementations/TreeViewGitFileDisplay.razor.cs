namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

public partial class TreeViewGitFileDisplay
    : FluxorComponent, ITreeViewGitFileRendererType
{
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;

    [CascadingParameter]
    public TreeViewState TreeViewState { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeViewGitFile TreeViewGitFile { get; set; } = null!;
}
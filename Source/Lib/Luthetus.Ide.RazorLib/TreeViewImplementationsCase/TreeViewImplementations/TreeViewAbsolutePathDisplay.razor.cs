using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types.TreeViews;
using Luthetus.Ide.ClassLib.TreeViewImplementationsCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

public partial class TreeViewAbsolutePathDisplay : ComponentBase,
    ITreeViewAbsolutePathRendererType
{
    [CascadingParameter]
    public TreeViewState TreeViewState { get; set; } = null!;
    [CascadingParameter(Name = "SearchQuery")]
    public string SearchQuery { get; set; } = string.Empty;
    [CascadingParameter(Name = "SearchMatchTuples")]
    public List<(TreeViewStateKey treeViewStateKey, TreeViewAbsolutePath treeViewAbsolutePath)>? SearchMatchTuples { get; set; }

    [Parameter, EditorRequired]
    public TreeViewAbsolutePath TreeViewAbsolutePath { get; set; } = null!;
}
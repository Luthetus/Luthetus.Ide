using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Types.TreeViews;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.TreeViewImplementations;

public partial class TreeViewNamespacePathDisplay
    : ComponentBase, ITreeViewNamespacePathRendererType
{
    [Parameter, EditorRequired]
    public NamespacePath NamespacePath { get; set; } = null!;
}
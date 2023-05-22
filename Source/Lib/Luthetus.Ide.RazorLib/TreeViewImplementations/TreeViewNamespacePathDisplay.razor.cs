using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Luthetus.Ide.ClassLib.Namespaces;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

public partial class TreeViewNamespacePathDisplay
    : ComponentBase, ITreeViewNamespacePathRendererType
{
    [Parameter, EditorRequired]
    public NamespacePath NamespacePath { get; set; } = null!;
}
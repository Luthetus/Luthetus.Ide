using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Ide.ClassLib.TreeViewImplementations.CompilerServiceExplorerCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.CompilerServiceExplorerCase;

public partial class TreeViewCSharpBinderDisplay : ComponentBase, ITreeViewCSharpBinderRendererType
{
    [Parameter, EditorRequired]
    public TreeViewCSharpBinder TreeViewCSharpBinder { get; set; } = null!;
}
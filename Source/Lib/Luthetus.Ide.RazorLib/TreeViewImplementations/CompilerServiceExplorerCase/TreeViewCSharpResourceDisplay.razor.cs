using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Ide.ClassLib.TreeViewImplementations.CompilerServiceExplorerCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.CompilerServiceExplorerCase;

public partial class TreeViewCSharpResourceDisplay : ComponentBase, ITreeViewCSharpResourceRendererType
{
    [Parameter, EditorRequired]
    public TreeViewCSharpResource TreeViewCSharpResource { get; set; } = null!;
}
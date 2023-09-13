using Luthetus.Ide.ClassLib.ComponentRenderers.Types.TreeViews;
using Luthetus.Ide.ClassLib.NugetCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

public partial class TreeViewCSharpProjectNugetPackageReferenceDisplay :
    ComponentBase, ITreeViewCSharpProjectNugetPackageReferenceRendererType
{
    [Parameter, EditorRequired]
    public CSharpProjectNugetPackageReference CSharpProjectNugetPackageReference { get; set; } = null!;
}
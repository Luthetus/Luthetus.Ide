using Luthetus.Ide.RazorLib.ComponentRenderersCase.Types.TreeViews;
using Luthetus.Ide.RazorLib.NugetCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.TreeViewImplementations;

public partial class TreeViewCSharpProjectNugetPackageReferenceDisplay :
    ComponentBase, ITreeViewCSharpProjectNugetPackageReferenceRendererType
{
    [Parameter, EditorRequired]
    public CSharpProjectNugetPackageReference CSharpProjectNugetPackageReference { get; set; } = null!;
}
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.Ide.RazorLib.NugetCase.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Displays;

public partial class TreeViewCSharpProjectNugetPackageReferenceDisplay :
    ComponentBase, ITreeViewCSharpProjectNugetPackageReferenceRendererType
{
    [Parameter, EditorRequired]
    public CSharpProjectNugetPackageReference CSharpProjectNugetPackageReference { get; set; } = null!;
}
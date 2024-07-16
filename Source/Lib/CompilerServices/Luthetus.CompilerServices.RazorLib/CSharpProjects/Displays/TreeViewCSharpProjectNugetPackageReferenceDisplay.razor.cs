using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.CompilerServices.RazorLib.Nugets.Models;
using Luthetus.CompilerServices.RazorLib.ComponentRenderers.Models;

namespace Luthetus.CompilerServices.RazorLib.CSharpProjects.Displays;

public partial class TreeViewCSharpProjectNugetPackageReferenceDisplay : ComponentBase, ITreeViewCSharpProjectNugetPackageReferenceRendererType
{
    [Parameter, EditorRequired]
    public CSharpProjectNugetPackageReference CSharpProjectNugetPackageReference { get; set; } = null!;
}
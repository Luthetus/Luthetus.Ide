using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Luthetus.Ide.ClassLib.Nuget;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

public partial class TreeViewCSharpProjectNugetPackageReferenceDisplay :
    ComponentBase, ITreeViewCSharpProjectNugetPackageReferenceRendererType
{
    [Parameter, EditorRequired]
    public CSharpProjectNugetPackageReference CSharpProjectNugetPackageReference { get; set; } = null!;
}
using Microsoft.AspNetCore.Components;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.CompilerServices.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.CompilerServices.RazorLib.CSharpProjects.Displays;

public partial class TreeViewCSharpProjectToProjectReferenceDisplay : ComponentBase, ITreeViewCSharpProjectToProjectReferenceRendererType
{
    [Parameter, EditorRequired]
    public CSharpProjectToProjectReference CSharpProjectToProjectReference { get; set; } = null!;
}
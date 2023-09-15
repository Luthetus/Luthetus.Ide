using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Displays;

public partial class TreeViewCSharpProjectToProjectReferenceDisplay :
    ComponentBase, ITreeViewCSharpProjectToProjectReferenceRendererType
{
    [Parameter, EditorRequired]
    public CSharpProjectToProjectReference CSharpProjectToProjectReference { get; set; } = null!;
}
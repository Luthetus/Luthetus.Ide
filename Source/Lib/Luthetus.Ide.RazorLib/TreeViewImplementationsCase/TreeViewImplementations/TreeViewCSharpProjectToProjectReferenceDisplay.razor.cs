using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Types.TreeViews;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.TreeViewImplementations;

public partial class TreeViewCSharpProjectToProjectReferenceDisplay :
    ComponentBase, ITreeViewCSharpProjectToProjectReferenceRendererType
{
    [Parameter, EditorRequired]
    public CSharpProjectToProjectReference CSharpProjectToProjectReference { get; set; } = null!;
}
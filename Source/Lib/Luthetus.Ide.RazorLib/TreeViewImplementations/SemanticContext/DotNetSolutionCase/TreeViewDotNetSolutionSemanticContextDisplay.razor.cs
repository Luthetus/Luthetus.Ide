using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Implementations;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.DotNetSolutionCase;

public partial class TreeViewDotNetSolutionSemanticContextDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public DotNetSolutionSemanticContext DotNetSolutionSemanticContext { get; set; } = null!;
}
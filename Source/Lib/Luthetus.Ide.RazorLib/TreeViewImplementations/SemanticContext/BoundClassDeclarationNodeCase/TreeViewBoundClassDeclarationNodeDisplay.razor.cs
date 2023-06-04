using Microsoft.AspNetCore.Components;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.BoundClassDeclarationNodeCase;

public partial class TreeViewBoundClassDeclarationNodeDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public BoundClassDeclarationNode BoundClassDeclarationNode { get; set; } = null!;
}
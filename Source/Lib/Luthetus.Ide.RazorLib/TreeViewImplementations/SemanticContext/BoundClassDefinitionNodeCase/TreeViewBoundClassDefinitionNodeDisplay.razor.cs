using Microsoft.AspNetCore.Components;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.BoundClassDefinitionNodeCase;

public partial class TreeViewBoundClassDefinitionNodeDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public BoundClassDefinitionNode BoundClassDefinitionNode { get; set; } = null!;
}
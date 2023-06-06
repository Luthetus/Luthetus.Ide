using Microsoft.AspNetCore.Components;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.NamespaceCase;

public partial class TreeViewNamespaceDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public BoundNamespaceStatementNode BoundNamespaceStatementNode { get; set; } = null!;
}
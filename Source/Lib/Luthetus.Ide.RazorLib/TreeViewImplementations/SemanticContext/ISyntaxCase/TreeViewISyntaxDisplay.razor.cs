using Microsoft.AspNetCore.Components;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.ISyntaxCase;

public partial class TreeViewISyntaxDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ISyntax Syntax { get; set; } = null!;
}
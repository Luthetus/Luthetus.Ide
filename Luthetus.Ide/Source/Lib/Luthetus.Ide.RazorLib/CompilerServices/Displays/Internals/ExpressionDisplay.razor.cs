using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays.Internals;

public partial class ExpressionDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public IExpressionNode ExpressionNode { get; set; } = null!;
}
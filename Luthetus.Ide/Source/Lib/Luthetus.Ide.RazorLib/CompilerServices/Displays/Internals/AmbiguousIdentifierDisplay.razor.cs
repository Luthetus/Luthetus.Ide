using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays.Internals;

public partial class AmbiguousIdentifierDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ISyntaxNode SyntaxNode { get; set; } = null!;
}
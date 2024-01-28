using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays.Internals;

public partial class TokenDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ISyntaxToken SyntaxToken { get; set; } = null!;
}
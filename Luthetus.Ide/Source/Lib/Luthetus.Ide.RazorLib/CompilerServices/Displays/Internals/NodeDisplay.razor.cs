using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays.Internals;

/// <summary>
/// This component encapsulates all the conditional branches to render the corresponding
/// <see cref="IfStatementDisplay"/>, <see cref="PropertyDisplay"/>, <see cref="TypeDisplay"/>,
/// etc... for the given node.
/// </summary>
public partial class NodeDisplay
{
    [Parameter, EditorRequired]
    public ISyntaxNode SyntaxNode { get; set; } = null!;
}
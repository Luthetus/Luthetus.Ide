using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays.Internals;

public partial class TypeDisplay : ComponentBase
{
    [CascadingParameter, EditorRequired]
    public CSharpCompilerService CSharpCompilerService { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public CSharpResource CSharpResource { get; set; } = null!;

    [Parameter, EditorRequired]
    public ISyntaxNode SyntaxNode { get; set; } = null!;
}
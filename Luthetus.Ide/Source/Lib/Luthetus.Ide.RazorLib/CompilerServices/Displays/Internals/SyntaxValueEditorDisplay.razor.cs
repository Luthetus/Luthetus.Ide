using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays.Internals;

public partial class SyntaxValueEditorDisplay : ComponentBase
{
    [CascadingParameter, EditorRequired]
    public CSharpCompilerService CSharpCompilerService { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public CSharpResource CSharpResource { get; set; } = null!;

    [Parameter, EditorRequired]
    public ISyntax Syntax { get; set; } = null!;
    [Parameter, EditorRequired]
    public PropertyInfo PropertyInfo { get; set; } = null!;

    private Guid _htmlIdGuid = Guid.NewGuid();
    private string HtmlId => $"luth_ide_syntax-editor-input_{_htmlIdGuid}";
}
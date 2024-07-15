using System.Reflection;
using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays.Internals;

public partial class SyntaxDisplay : ComponentBase
{
    [Inject]
    private IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;

    [CascadingParameter, EditorRequired]
    public CSharpCompilerService CSharpCompilerService { get; set; } = null!;
    [CascadingParameter, EditorRequired]
    public CSharpResource CSharpResource { get; set; } = null!;

    [Parameter, EditorRequired]
    public ISyntax Syntax { get; set; } = null!;

    /// <summary>
    /// Use the object's type full name as the key. This way the
    /// property info lists can be cached by data type.
    /// </summary>
    private static readonly Dictionary<string, PropertyInfo[]> _propertyInfoMap = new();
}
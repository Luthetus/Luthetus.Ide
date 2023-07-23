using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceCase;

public partial class CompilerServiceResourceDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ICompilerServiceResource CompilerServiceResource { get; set; } = null!;
}
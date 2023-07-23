using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceCase;

public partial class DotNetSolutionResourceDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public DotNetSolutionResource DotNetSolutionResource { get; set; } = null!;
}
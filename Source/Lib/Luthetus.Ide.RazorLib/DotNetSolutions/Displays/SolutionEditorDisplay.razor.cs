using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Displays;

public partial class SolutionEditorDisplay : ComponentBase
{
    [Inject]
    private DotNetSolutionCompilerService DotNetSolutionCompilerService { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<DotNetSolutionModel> DotNetSolutionModelKey { get; set; }
    [Parameter, EditorRequired]
    public ResourceUri DotNetSolutionResourceUri { get; set; } = null!;
}
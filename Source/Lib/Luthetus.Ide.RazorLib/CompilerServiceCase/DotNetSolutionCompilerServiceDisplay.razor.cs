using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceCase;

public partial class DotNetSolutionCompilerServiceDisplay : ComponentBase
{
    [Inject]
    private DotNetSolutionCompilerService DotNetCompilerService { get; set; } = null!;
}
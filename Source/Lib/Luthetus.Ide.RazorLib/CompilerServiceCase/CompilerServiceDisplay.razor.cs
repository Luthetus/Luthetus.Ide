using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceCase;

public partial class CompilerServiceDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ICompilerService CompilerService { get; set; } = null!;
    [Parameter, EditorRequired]
    public int Depth { get; set; } = -1;

    private CompilerServiceDisplayDimensions _compilerServiceDisplayDimensions = new();

    private async Task OnDimensionsStateHasChanged(CompilerServiceDisplayDimensions compilerServiceDisplayDimensions)
    {
        _compilerServiceDisplayDimensions = compilerServiceDisplayDimensions;
        await InvokeAsync(StateHasChanged);
    }
}
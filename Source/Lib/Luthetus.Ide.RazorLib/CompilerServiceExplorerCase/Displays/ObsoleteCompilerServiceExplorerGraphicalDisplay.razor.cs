using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Models;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Displays;

public partial class ObsoleteCompilerServiceExplorerGraphicalDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ICompilerService CompilerService { get; set; } = null!;
    [Parameter, EditorRequired]
    public int Depth { get; set; } = -1;

    private ObsoleteCompilerServiceGraphicalDimensions _compilerServiceDisplayDimensions = new();

    private async Task OnDimensionsStateHasChanged(ObsoleteCompilerServiceGraphicalDimensions compilerServiceDisplayDimensions)
    {
        _compilerServiceDisplayDimensions = compilerServiceDisplayDimensions;
        await InvokeAsync(StateHasChanged);
    }
}
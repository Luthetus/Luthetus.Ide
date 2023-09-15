using Fluxor;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Displays;

public partial class CompilerServiceExplorerGraphicalDisplay : ComponentBase
{
    [Inject]
    private IState<CompilerServiceExplorerRegistry> CompilerServiceExplorerStateWrap { get; set; } = null!;
}
using Fluxor;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Displays;

public partial class CompilerServiceExplorerGraphicalDisplay : ComponentBase
{
    [Inject]
    private IState<CompilerServiceExplorerRegistry> CompilerServiceExplorerStateWrap { get; set; } = null!;
}
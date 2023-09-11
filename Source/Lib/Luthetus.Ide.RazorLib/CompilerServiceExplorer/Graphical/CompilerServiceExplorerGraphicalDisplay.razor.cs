using Fluxor;
using Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorer.Graphical;

public partial class CompilerServiceExplorerGraphicalDisplay : ComponentBase
{
    [Inject]
    private IState<CompilerServiceExplorerRegistry> CompilerServiceExplorerStateWrap { get; set; } = null!;
}
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record TerminalGroupState
{
    public record SetActiveTerminalAction(Key<ITerminal> TerminalKey);
    
    public record InitializeResizeHandleDimensionUnitAction(DimensionUnit DimensionUnit);
}

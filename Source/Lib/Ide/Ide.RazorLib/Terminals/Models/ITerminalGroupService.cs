using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public interface ITerminalGroupService
{
	public event Action? TerminalGroupStateChanged;
	
	public TerminalGroupState GetTerminalGroupState();

    public void ReduceSetActiveTerminalAction(Key<ITerminal> terminalKey);
    public void ReduceInitializeResizeHandleDimensionUnitAction(DimensionUnit dimensionUnit);
}

using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public interface ITerminalGroupService
{
	public event Action? TerminalGroupStateChanged;
	
	public TerminalGroupState GetTerminalGroupState();

    public void SetActiveTerminal(Key<ITerminal> terminalKey);
    public void InitializeResizeHandleDimensionUnit(DimensionUnit dimensionUnit);
}

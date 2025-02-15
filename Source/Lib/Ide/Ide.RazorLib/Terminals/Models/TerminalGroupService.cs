using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalGroupService : ITerminalGroupService
{
    private readonly object _stateModificationLock = new();

    private TerminalGroupState _terminalGroupState = new();
	
	public event Action? TerminalGroupStateChanged;
	
	public TerminalGroupState GetTerminalGroupState() => _terminalGroupState;

    public void SetActiveTerminal(Key<ITerminal> terminalKey)
    {
        lock (_stateModificationLock)
        {
            var inState = GetTerminalGroupState();

            _terminalGroupState = inState with
            {
                ActiveTerminalKey = terminalKey
            };

            goto finalize;
        }

        finalize:
        TerminalGroupStateChanged?.Invoke();
    }
    
    public void InitializeResizeHandleDimensionUnit(DimensionUnit dimensionUnit)
    {
        lock (_stateModificationLock)
        {
            var inState = GetTerminalGroupState();

            if (dimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN)
                goto finalize;

            // BodyElementDimensions
            {
                if (inState.BodyElementDimensions.WidthDimensionAttribute.DimensionUnitList is null)
                    goto finalize;

                var existingDimensionUnit = inState.BodyElementDimensions.WidthDimensionAttribute.DimensionUnitList
                    .FirstOrDefault(x => x.Purpose == dimensionUnit.Purpose);

                if (existingDimensionUnit.Purpose is not null)
                    goto finalize;

                inState.BodyElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(dimensionUnit);
            }

            // TabsElementDimensions
            {
                if (inState.TabsElementDimensions.WidthDimensionAttribute.DimensionUnitList is null)
                    goto finalize;

                var existingDimensionUnit = inState.TabsElementDimensions.WidthDimensionAttribute.DimensionUnitList
                    .FirstOrDefault(x => x.Purpose == dimensionUnit.Purpose);

                if (existingDimensionUnit.Purpose is not null)
                    goto finalize;

                inState.TabsElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(dimensionUnit);
            }

            goto finalize;
        }

        finalize:
        TerminalGroupStateChanged?.Invoke();
    }
}

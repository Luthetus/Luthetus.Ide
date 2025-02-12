using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalGroupService : ITerminalGroupService
{
	private TerminalGroupState _terminalGroupState = new();
	
	public event Action? TerminalGroupStateChanged;
	
	public TerminalGroupState GetTerminalGroupState() => _terminalGroupState;

    public void ReduceSetActiveTerminalAction(Key<ITerminal> terminalKey)
    {
    	var inState = GetTerminalGroupState();
    
        _terminalGroupState = inState with
        {
            ActiveTerminalKey = terminalKey
        };
        
        TerminalGroupStateChanged?.Invoke();
        return;
    }
    
    public void ReduceInitializeResizeHandleDimensionUnitAction(DimensionUnit dimensionUnit)
    {
    	var inState = GetTerminalGroupState();
    
        if (dimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN)
        {
        	TerminalGroupStateChanged?.Invoke();
        	return;
        }
        
        // BodyElementDimensions
        {
        	if (inState.BodyElementDimensions.WidthDimensionAttribute.DimensionUnitList is null)
        	{
        		TerminalGroupStateChanged?.Invoke();
        		return;
        	}
        		
        	var existingDimensionUnit = inState.BodyElementDimensions.WidthDimensionAttribute.DimensionUnitList
        		.FirstOrDefault(x => x.Purpose == dimensionUnit.Purpose);
        		
            if (existingDimensionUnit.Purpose is not null)
            {
            	TerminalGroupStateChanged?.Invoke();
        		return;
            }
        		
        	inState.BodyElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(dimensionUnit);
        }
        
        // TabsElementDimensions
        {
        	if (inState.TabsElementDimensions.WidthDimensionAttribute.DimensionUnitList is null)
        	{
        		TerminalGroupStateChanged?.Invoke();
        		return;
        	}
        		
        	var existingDimensionUnit = inState.TabsElementDimensions.WidthDimensionAttribute.DimensionUnitList
        		.FirstOrDefault(x => x.Purpose == dimensionUnit.Purpose);
        		
            if (existingDimensionUnit.Purpose is not null)
            {
            	TerminalGroupStateChanged?.Invoke();
        		return;
            }
        		
        	inState.TabsElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(dimensionUnit);
        }
        
        TerminalGroupStateChanged?.Invoke();
        return;
    }
}

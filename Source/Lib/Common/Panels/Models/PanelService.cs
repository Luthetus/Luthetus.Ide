using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.RazorLib.Panels.Models;

public class PanelService : IPanelService
{
	private readonly IAppDimensionService _appDimensionService;

	public PanelService(IAppDimensionService appDimensionService)
	{
		_appDimensionService = appDimensionService;
	}

	private PanelState _panelState = new();
	
	public event Action? PanelStateChanged;
	
	public PanelState GetPanelState() => _panelState;

    public void ReduceRegisterPanelAction(Panel panel)
    {
    	var inState = GetPanelState();
    
        if (inState.PanelList.Any(x => x.Key == panel.Key))
        {
            PanelStateChanged?.Invoke();
            return;
        }

        var outPanelList = new List<Panel>(inState.PanelList);
        outPanelList.Add(panel);

        _panelState = inState with { PanelList = outPanelList };
        PanelStateChanged?.Invoke();
        return;
    }

    public void ReduceDisposePanelAction(Key<Panel> panelKey)
    {
    	var inState = GetPanelState();
    
        var indexPanel = inState.PanelList.FindIndex(
            x => x.Key == panelKey);

        if (indexPanel == -1)
        {
            PanelStateChanged?.Invoke();
        	return;
        }

        var outPanelList = new List<Panel>(inState.PanelList);
        outPanelList.RemoveAt(indexPanel);

        _panelState = inState with { PanelList = outPanelList };
        PanelStateChanged?.Invoke();
        return;
    }
	
    public void ReduceRegisterPanelGroupAction(PanelGroup panelGroup)
    {
    	var inState = GetPanelState();
    
        if (inState.PanelGroupList.Any(x => x.Key == panelGroup.Key))
        {
            PanelStateChanged?.Invoke();
       	 return;
        }

        var outPanelGroupList = new List<PanelGroup>(inState.PanelGroupList);
        outPanelGroupList.Add(panelGroup);
        
        _panelState = inState with { PanelGroupList = outPanelGroupList };
        PanelStateChanged?.Invoke();
        return;
    }

    public void ReduceDisposePanelGroupAction(Key<PanelGroup> panelGroupKey)
    {
    	var inState = GetPanelState();
    
        var indexPanelGroup = inState.PanelGroupList.FindIndex(
            x => x.Key == panelGroupKey);

        if (indexPanelGroup == -1)
        {
            PanelStateChanged?.Invoke();
        	return;
        }

        var outPanelGroupList = new List<PanelGroup>(inState.PanelGroupList);
        outPanelGroupList.RemoveAt(indexPanelGroup);

        _panelState = inState with { PanelGroupList = outPanelGroupList };
        PanelStateChanged?.Invoke();
        return;
    }

    public void ReduceRegisterPanelTabAction(
    	Key<PanelGroup> panelGroupKey,
    	IPanelTab panelTab,
    	bool insertAtIndexZero)
    {
    	var inState = GetPanelState();
    
        var indexPanelGroup = inState.PanelGroupList.FindIndex(
            x => x.Key == panelGroupKey);

        if (indexPanelGroup == -1)
        {
            PanelStateChanged?.Invoke();
        	return;
        }
            
        var inPanelGroup = inState.PanelGroupList[indexPanelGroup];

        var outTabList = new List<IPanelTab>(inPanelGroup.TabList);

        var insertionPoint = insertAtIndexZero
            ? 0
            : outTabList.Count;

        outTabList.Insert(insertionPoint, panelTab);

        var outPanelGroupList = new List<PanelGroup>(inState.PanelGroupList);
        
        outPanelGroupList[indexPanelGroup] = inPanelGroup with
        {
            TabList = outTabList
        };

        _panelState = inState with
        {
            PanelGroupList = outPanelGroupList
        };
        
        PanelStateChanged?.Invoke();
        return;
    }

    public void ReduceDisposePanelTabAction(Key<PanelGroup> panelGroupKey, Key<Panel> panelTabKey)
    {
    	var inState = GetPanelState();
    
        var indexPanelGroup = inState.PanelGroupList.FindIndex(
            x => x.Key == panelGroupKey);

        if (indexPanelGroup == -1)
        {
            PanelStateChanged?.Invoke();
       	 return;
        }
            
        var inPanelGroup = inState.PanelGroupList[indexPanelGroup];

        var indexPanelTab = inPanelGroup.TabList.FindIndex(
            x => x.Key == panelTabKey);

        if (indexPanelTab == -1)
        {
            PanelStateChanged?.Invoke();
       	 return;
        }

        var outTabList = new List<IPanelTab>(inPanelGroup.TabList);
        outTabList.RemoveAt(indexPanelTab);
        
        var outPanelGroupList = new List<PanelGroup>(inState.PanelGroupList);
        outPanelGroupList[indexPanelGroup] = inPanelGroup with
        {
            TabList = outTabList
        };

        _panelState = inState with { PanelGroupList = outPanelGroupList };
        PanelStateChanged?.Invoke();
        return;
    }

    public void ReduceSetActivePanelTabAction(Key<PanelGroup> panelGroupKey, Key<Panel> panelTabKey)
    {
    	var inState = GetPanelState();
    
        var indexPanelGroup = inState.PanelGroupList.FindIndex(
            x => x.Key == panelGroupKey);

        if (indexPanelGroup == -1)
        {
            PanelStateChanged?.Invoke();
        	return;
        }
            
        var inPanelGroup = inState.PanelGroupList[indexPanelGroup];

		var outPanelGroupList = new List<PanelGroup>(inState.PanelGroupList);
		
        outPanelGroupList[indexPanelGroup] = inPanelGroup with
        {
            ActiveTabKey = panelTabKey
        };

        _panelState = inState with { PanelGroupList = outPanelGroupList };
        PanelStateChanged?.Invoke();
        
        _appDimensionService.ReduceNotifyIntraAppResizeAction();
        return;
    }

    public void ReduceSetPanelTabAsActiveByContextRecordKeyAction(Key<ContextRecord> contextRecordKey)
    {
    	var inState = GetPanelState();
    
        var inPanelGroup = inState.PanelGroupList.FirstOrDefault(x => x.TabList
            .Any(y => y.ContextRecordKey == contextRecordKey));

        if (inPanelGroup is null)
        {
            PanelStateChanged?.Invoke();
        	return;
        }

        var inPanelTab = inPanelGroup.TabList.FirstOrDefault(
            x => x.ContextRecordKey == contextRecordKey);

        if (inPanelTab is null)
        {
            PanelStateChanged?.Invoke();
        	return;
        }

        ReduceSetActivePanelTabAction(inPanelGroup.Key, inPanelTab.Key);
        return;
    }

    public void ReduceSetDragEventArgsAction((IPanelTab PanelTab, PanelGroup PanelGroup)? dragEventArgs)
    {
    	var inState = GetPanelState();
    
        _panelState = inState with
        {
            DragEventArgs = dragEventArgs
        };
        
        PanelStateChanged?.Invoke();
        return;
    }
    
    public void ReduceInitializeResizeHandleDimensionUnitAction(Key<PanelGroup> panelGroupKey, DimensionUnit dimensionUnit)
    {
    	var inState = GetPanelState();
    
        var inPanelGroup = inState.PanelGroupList.FirstOrDefault(
            x => x.Key == panelGroupKey);

        if (inPanelGroup is null)
        {
            PanelStateChanged?.Invoke();
        	return;
        }
            
        if (dimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_ROW ||
        	dimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN)
        {
        	PanelStateChanged?.Invoke();
        	return;
        }
            
		if (dimensionUnit.Purpose == DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_ROW)
        {
        	if (inPanelGroup.ElementDimensions.HeightDimensionAttribute.DimensionUnitList is null)
        	{
        		PanelStateChanged?.Invoke();
        		return;
       	 }
        		
			var existingDimensionUnit = inPanelGroup.ElementDimensions.HeightDimensionAttribute.DimensionUnitList
				.FirstOrDefault(x => x.Purpose == dimensionUnit.Purpose);
				
            if (existingDimensionUnit.Purpose is not null)
            {
            	PanelStateChanged?.Invoke();
        		return;
            }
        		
        	inPanelGroup.ElementDimensions.HeightDimensionAttribute.DimensionUnitList.Add(dimensionUnit);
        }
        else if (dimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN)
        {
        	if (inPanelGroup.ElementDimensions.WidthDimensionAttribute.DimensionUnitList is null)
        	{
        		PanelStateChanged?.Invoke();
        		return;
        	}
        		
        	var existingDimensionUnit = inPanelGroup.ElementDimensions.WidthDimensionAttribute.DimensionUnitList
        		.FirstOrDefault(x => x.Purpose == dimensionUnit.Purpose);
            
            if (existingDimensionUnit.Purpose is not null)
            {
            	PanelStateChanged?.Invoke();
        		return;
            }
        		
        	inPanelGroup.ElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(dimensionUnit);
        }
        
        PanelStateChanged?.Invoke();
        return;
    }
}

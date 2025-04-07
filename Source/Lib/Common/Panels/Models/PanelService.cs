using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.ListExtensions;

namespace Luthetus.Common.RazorLib.Panels.Models;

public class PanelService : IPanelService
{
    private readonly object _stateModificationLock = new();

    private readonly IAppDimensionService _appDimensionService;

	public PanelService(IAppDimensionService appDimensionService)
	{
		_appDimensionService = appDimensionService;
	}

	private PanelState _panelState = new();
	
	public event Action? PanelStateChanged;
	
	public PanelState GetPanelState() => _panelState;

    public void RegisterPanel(Panel panel)
    {
        lock (_stateModificationLock)
        {
    	    var inState = GetPanelState();
    
            if (inState.PanelList.Any(x => x.Key == panel.Key))
                goto finalize;

            var outPanelList = new List<Panel>(inState.PanelList);
            outPanelList.Add(panel);

            _panelState = inState with { PanelList = outPanelList };
            goto finalize;
        }

        finalize:
        PanelStateChanged?.Invoke();
    }

    public void DisposePanel(Key<Panel> panelKey)
    {
        lock (_stateModificationLock)
        {
            var inState = GetPanelState();

            var indexPanel = inState.PanelList.FindIndex(
                x => x.Key == panelKey);

            if (indexPanel == -1)
                goto finalize;

            var outPanelList = new List<Panel>(inState.PanelList);
            outPanelList.RemoveAt(indexPanel);

            _panelState = inState with { PanelList = outPanelList };
            goto finalize;
        }

        finalize:
        PanelStateChanged?.Invoke();
    }
	
    public void RegisterPanelGroup(PanelGroup panelGroup)
    {
        lock (_stateModificationLock)
        {
            var inState = GetPanelState();

            if (inState.PanelGroupList.Any(x => x.Key == panelGroup.Key))
                goto finalize;

            var outPanelGroupList = new List<PanelGroup>(inState.PanelGroupList);
            outPanelGroupList.Add(panelGroup);

            _panelState = inState with { PanelGroupList = outPanelGroupList };
            goto finalize;
        }

        finalize:
        PanelStateChanged?.Invoke();
    }

    public void DisposePanelGroup(Key<PanelGroup> panelGroupKey)
    {
        lock (_stateModificationLock)
        {
            var inState = GetPanelState();

            var indexPanelGroup = inState.PanelGroupList.FindIndex(
                x => x.Key == panelGroupKey);

            if (indexPanelGroup == -1)
                goto finalize;

            var outPanelGroupList = new List<PanelGroup>(inState.PanelGroupList);
            outPanelGroupList.RemoveAt(indexPanelGroup);

            _panelState = inState with { PanelGroupList = outPanelGroupList };
            goto finalize;
        }

        finalize:
        PanelStateChanged?.Invoke();
    }

    public void RegisterPanelTab(
    	Key<PanelGroup> panelGroupKey,
    	IPanelTab panelTab,
    	bool insertAtIndexZero)
    {
        lock (_stateModificationLock)
        {
            var inState = GetPanelState();

            var indexPanelGroup = inState.PanelGroupList.FindIndex(
                x => x.Key == panelGroupKey);

            if (indexPanelGroup == -1)
                goto finalize;

            var inPanelGroup = inState.PanelGroupList[indexPanelGroup];
            
            if (inPanelGroup.TabList.Any(x => x.Key == panelTab.Key))
            	goto finalize;

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

            goto finalize;
        }

        finalize:
        PanelStateChanged?.Invoke();
    }

    public void DisposePanelTab(Key<PanelGroup> panelGroupKey, Key<Panel> panelTabKey)
    {
        lock (_stateModificationLock)
        {
            var inState = GetPanelState();

            var indexPanelGroup = inState.PanelGroupList.FindIndex(
                x => x.Key == panelGroupKey);

            if (indexPanelGroup == -1)
                goto finalize;

            var inPanelGroup = inState.PanelGroupList[indexPanelGroup];

            var indexPanelTab = inPanelGroup.TabList.FindIndex(
                x => x.Key == panelTabKey);

            if (indexPanelTab == -1)
                goto finalize;

            var outTabList = new List<IPanelTab>(inPanelGroup.TabList);
            outTabList.RemoveAt(indexPanelTab);

            var outPanelGroupList = new List<PanelGroup>(inState.PanelGroupList);
            outPanelGroupList[indexPanelGroup] = inPanelGroup with
            {
                TabList = outTabList
            };

            _panelState = inState with { PanelGroupList = outPanelGroupList };
            goto finalize;
        }

        finalize:
        PanelStateChanged?.Invoke();
    }

    public void SetActivePanelTab(Key<PanelGroup> panelGroupKey, Key<Panel> panelTabKey)
    {
        var sideEffect = false;

        lock (_stateModificationLock)
        {
            var inState = GetPanelState();

            var indexPanelGroup = inState.PanelGroupList.FindIndex(
                x => x.Key == panelGroupKey);

            if (indexPanelGroup == -1)
                goto finalize;

            var inPanelGroup = inState.PanelGroupList[indexPanelGroup];

            var outPanelGroupList = new List<PanelGroup>(inState.PanelGroupList);

            outPanelGroupList[indexPanelGroup] = inPanelGroup with
            {
                ActiveTabKey = panelTabKey
            };

            _panelState = inState with { PanelGroupList = outPanelGroupList };
            sideEffect = true;
            goto finalize;
        }

        finalize:
        PanelStateChanged?.Invoke();

        if (sideEffect)
            _appDimensionService.NotifyIntraAppResize();
    }

    public void SetPanelTabAsActiveByContextRecordKey(Key<ContextRecord> contextRecordKey)
    {
        lock (_stateModificationLock)
        {
            var inState = GetPanelState();

            var inPanelGroup = inState.PanelGroupList.FirstOrDefault(x => x.TabList
                .Any(y => y.ContextRecordKey == contextRecordKey));

            if (inPanelGroup is null)
                goto finalize;

            var inPanelTab = inPanelGroup.TabList.FirstOrDefault(
                x => x.ContextRecordKey == contextRecordKey);

            if (inPanelTab is null)
                goto finalize;

            // TODO: This should be thread safe yes?...
            // ...Only ever would the same thread access the inner lock from invoking this which is the current lock so no deadlock?
            SetActivePanelTab(inPanelGroup.Key, inPanelTab.Key);
            return; // Inner reduce will trigger finalize.
        }

        finalize:
        PanelStateChanged?.Invoke();
    }

    public void SetDragEventArgs((IPanelTab PanelTab, PanelGroup PanelGroup)? dragEventArgs)
    {
        lock (_stateModificationLock)
        {
            var inState = GetPanelState();
    
            _panelState = inState with
            {
                DragEventArgs = dragEventArgs
            };

            goto finalize;
        }

        finalize:
        PanelStateChanged?.Invoke();
    }
    
    public void InitializeResizeHandleDimensionUnit(Key<PanelGroup> panelGroupKey, DimensionUnit dimensionUnit)
    {
        lock (_stateModificationLock)
        {
            var inState = GetPanelState();

            var inPanelGroup = inState.PanelGroupList.FirstOrDefault(
                x => x.Key == panelGroupKey);

            if (inPanelGroup is null)
                goto finalize;

            if (dimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_ROW ||
                dimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN)
            {
                goto finalize;
            }

            if (dimensionUnit.Purpose == DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_ROW)
            {
                if (inPanelGroup.ElementDimensions.HeightDimensionAttribute.DimensionUnitList is null)
                    goto finalize;

                var existingDimensionUnit = inPanelGroup.ElementDimensions.HeightDimensionAttribute.DimensionUnitList
                    .FirstOrDefault(x => x.Purpose == dimensionUnit.Purpose);

                if (existingDimensionUnit.Purpose is not null)
                    goto finalize;

                inPanelGroup.ElementDimensions.HeightDimensionAttribute.DimensionUnitList.Add(dimensionUnit);
            }
            else if (dimensionUnit.Purpose != DimensionUnitFacts.Purposes.RESIZABLE_HANDLE_COLUMN)
            {
                if (inPanelGroup.ElementDimensions.WidthDimensionAttribute.DimensionUnitList is null)
                    goto finalize;

                var existingDimensionUnit = inPanelGroup.ElementDimensions.WidthDimensionAttribute.DimensionUnitList
                    .FirstOrDefault(x => x.Purpose == dimensionUnit.Purpose);

                if (existingDimensionUnit.Purpose is not null)
                    goto finalize;

                inPanelGroup.ElementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(dimensionUnit);
            }

            goto finalize;
        }

        finalize:
        PanelStateChanged?.Invoke();
    }
}

using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.RazorLib.Panels.Models;

public interface IPanelService
{
	public event Action? PanelStateChanged;
	
	public PanelState GetPanelState();
	
	public void ReduceRegisterPanelAction(Panel panel);
    public void ReduceDisposePanelAction(Key<Panel> panelKey);
    public void ReduceRegisterPanelGroupAction(PanelGroup panelGroup);
    public void ReduceDisposePanelGroupAction(Key<PanelGroup> panelGroupKey);

    public void ReduceRegisterPanelTabAction(
    	Key<PanelGroup> panelGroupKey,
    	IPanelTab panelTab,
    	bool insertAtIndexZero);

    public void ReduceDisposePanelTabAction(Key<PanelGroup> panelGroupKey, Key<Panel> panelTabKey);
    public void ReduceSetActivePanelTabAction(Key<PanelGroup> panelGroupKey, Key<Panel> panelTabKey);
    public void ReduceSetPanelTabAsActiveByContextRecordKeyAction(Key<ContextRecord> contextRecordKey);
    public void ReduceSetDragEventArgsAction((IPanelTab PanelTab, PanelGroup PanelGroup)? dragEventArgs);
    public void ReduceInitializeResizeHandleDimensionUnitAction(Key<PanelGroup> panelGroupKey, DimensionUnit dimensionUnit);
}

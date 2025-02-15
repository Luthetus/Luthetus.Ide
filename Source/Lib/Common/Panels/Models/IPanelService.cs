using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.RazorLib.Panels.Models;

public interface IPanelService
{
	public event Action? PanelStateChanged;
	
	public PanelState GetPanelState();
	
	public void RegisterPanel(Panel panel);
    public void DisposePanel(Key<Panel> panelKey);
    public void RegisterPanelGroup(PanelGroup panelGroup);
    public void DisposePanelGroup(Key<PanelGroup> panelGroupKey);

    public void RegisterPanelTab(
    	Key<PanelGroup> panelGroupKey,
    	IPanelTab panelTab,
    	bool insertAtIndexZero);

    public void DisposePanelTab(Key<PanelGroup> panelGroupKey, Key<Panel> panelTabKey);
    public void SetActivePanelTab(Key<PanelGroup> panelGroupKey, Key<Panel> panelTabKey);
    public void SetPanelTabAsActiveByContextRecordKey(Key<ContextRecord> contextRecordKey);
    public void SetDragEventArgs((IPanelTab PanelTab, PanelGroup PanelGroup)? dragEventArgs);
    public void InitializeResizeHandleDimensionUnit(Key<PanelGroup> panelGroupKey, DimensionUnit dimensionUnit);
}

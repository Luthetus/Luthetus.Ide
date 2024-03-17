using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;

namespace Luthetus.Common.RazorLib.Panels.States;

public partial record PanelsState
{
    public record RegisterPanelAction(Panel Panel);
    public record DisposePanelAction(Key<Panel> PanelKey);

    public record RegisterPanelGroupAction(PanelGroup PanelGroup);
    public record DisposePanelGroupAction(Key<PanelGroup> PanelGroupKey);

    public record RegisterPanelTabAction(Key<PanelGroup> PanelGroupKey, IPanelTab PanelTab, bool InsertAtIndexZero);
    public record DisposePanelTabAction(Key<PanelGroup> PanelGroupKey, Key<Panel> PanelTabKey);

    public record SetActivePanelTabAction(Key<PanelGroup> PanelGroupKey, Key<Panel> PanelTabKey);
    public record SetPanelTabAsActiveByContextRecordKeyAction(Key<ContextRecord> ContextRecordKey);

    public record SetDragEventArgsAction((IPanelTab PanelTab, PanelGroup PanelGroup)? DragEventArgs);
}
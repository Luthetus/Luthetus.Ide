using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;

namespace Luthetus.Common.RazorLib.Panels.States;

public partial record PanelsStateTests
{
    public record RegisterPanelGroupAction(PanelGroup PanelGroup);
    public record DisposePanelGroupAction(Key<PanelGroup> PanelGroupKey);

    public record RegisterPanelTabAction(Key<PanelGroup> PanelGroupKey, PanelTab PanelTab, bool InsertAtIndexZero);
    public record DisposePanelTabAction(Key<PanelGroup> PanelGroupKey, Key<PanelTab> PanelTabKey);

    public record SetActivePanelTabAction(Key<PanelGroup> PanelGroupKey, Key<PanelTab> PanelTabKey);
    public record SetPanelTabAsActiveByContextRecordKeyAction(Key<ContextRecord> ContextRecordKey);

    public record SetDragEventArgsAction((PanelTab PanelTab, PanelGroup PanelGroup)? DragEventArgs);
}
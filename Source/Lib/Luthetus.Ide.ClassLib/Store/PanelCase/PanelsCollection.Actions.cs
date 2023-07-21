using Luthetus.Ide.ClassLib.Panel;

namespace Luthetus.Ide.ClassLib.Store.PanelCase;

public partial record PanelsCollection
{
    public record RegisterPanelRecordAction(PanelRecord PanelRecord);
    public record DisposePanelRecordAction(PanelRecordKey PanelRecordKey);

    public record RegisterPanelTabAction(PanelRecordKey PanelRecordKey, PanelTab PanelTab);
    public record DisposePanelTabAction(PanelRecordKey PanelRecordKey, PanelTabKey PanelTabKey);

    public record SetActivePanelTabAction(PanelRecordKey PanelRecordKey, PanelTabKey PanelTabKey);

    public record SetPanelDragEventArgsAction((PanelTab TagDragTarget, PanelRecord ParentPanelRecord)? PanelDragEventArgs);
}
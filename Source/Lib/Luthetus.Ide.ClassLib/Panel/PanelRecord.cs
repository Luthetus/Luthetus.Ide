namespace Luthetus.Ide.ClassLib.Panel;

public record PanelRecord(
    PanelRecordKey PanelRecordKey,
    PanelTabKey ActivePanelTabKey,
    ElementDimensions ElementDimensions,
    ImmutableArray<PanelTab> PanelTabs);
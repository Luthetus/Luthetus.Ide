using Luthetus.Ide.ClassLib.Store.PanelCase;

namespace Luthetus.Ide.ClassLib.Panel;

public static class PanelFacts
{
    public static readonly PanelRecordKey LeftPanelRecordKey = PanelRecordKey.NewPanelRecordKey();
    public static readonly PanelRecordKey RightPanelRecordKey = PanelRecordKey.NewPanelRecordKey();
    public static readonly PanelRecordKey BottomPanelRecordKey = PanelRecordKey.NewPanelRecordKey();

    public static PanelRecord GetLeftPanelRecord(PanelsCollection panelsCollection)
    {
        return panelsCollection.PanelRecordsList
            .First(x =>
                x.PanelRecordKey == LeftPanelRecordKey);
    }

    public static PanelRecord GetRightPanelRecord(PanelsCollection panelsCollection)
    {
        return panelsCollection.PanelRecordsList
            .First(x =>
                x.PanelRecordKey == RightPanelRecordKey);
    }

    public static PanelRecord GetBottomPanelRecord(PanelsCollection panelsCollection)
    {
        return panelsCollection.PanelRecordsList
            .First(x =>
                x.PanelRecordKey == BottomPanelRecordKey);
    }
}
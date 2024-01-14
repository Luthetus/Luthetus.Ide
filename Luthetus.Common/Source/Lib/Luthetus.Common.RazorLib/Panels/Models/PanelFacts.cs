using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.States;

namespace Luthetus.Common.RazorLib.Panels.Models;

public static class PanelFacts
{
    public static readonly Key<PanelGroup> LeftPanelRecordKey = Key<PanelGroup>.NewKey();
    public static readonly Key<PanelGroup> RightPanelRecordKey = Key<PanelGroup>.NewKey();
    public static readonly Key<PanelGroup> BottomPanelRecordKey = Key<PanelGroup>.NewKey();

    public static PanelGroup GetLeftPanelRecord(PanelsState panelsState)
    {
        return panelsState.PanelGroupList.First(x => x.Key == LeftPanelRecordKey);
    }

    public static PanelGroup GetRightPanelRecord(PanelsState panelsState)
    {
        return panelsState.PanelGroupList.First(x => x.Key == RightPanelRecordKey);
    }

    public static PanelGroup GetBottomPanelRecord(PanelsState panelsState)
    {
        return panelsState.PanelGroupList.First(x => x.Key == BottomPanelRecordKey);
    }
}
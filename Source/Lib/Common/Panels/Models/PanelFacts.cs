using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Panels.Models;

public static class PanelFacts
{
    public static readonly Key<PanelGroup> LeftPanelGroupKey = Key<PanelGroup>.NewKey();
    public static readonly Key<PanelGroup> RightPanelGroupKey = Key<PanelGroup>.NewKey();
    public static readonly Key<PanelGroup> BottomPanelGroupKey = Key<PanelGroup>.NewKey();

    public static PanelGroup GetTopLeftPanelGroup(PanelState panelState)
    {
        return panelState.PanelGroupList.First(x => x.Key == LeftPanelGroupKey);
    }

    public static PanelGroup GetTopRightPanelGroup(PanelState panelState)
    {
        return panelState.PanelGroupList.First(x => x.Key == RightPanelGroupKey);
    }

    public static PanelGroup GetBottomPanelGroup(PanelState panelState)
    {
        return panelState.PanelGroupList.First(x => x.Key == BottomPanelGroupKey);
    }
}
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.States;

namespace Luthetus.Common.RazorLib.Panels.Models;

public static class PanelFacts
{
    public static readonly Key<PanelGroup> LeftPanelGroupKey = Key<PanelGroup>.NewKey();
    public static readonly Key<PanelGroup> RightPanelGroupKey = Key<PanelGroup>.NewKey();
    public static readonly Key<PanelGroup> BottomPanelGroupKey = Key<PanelGroup>.NewKey();

    public static PanelGroup GetTopLeftPanelGroup(PanelsState panelsState)
    {
        return panelsState.PanelGroupList.First(x => x.Key == LeftPanelGroupKey);
    }

    public static PanelGroup GetTopRightPanelGroup(PanelsState panelsState)
    {
        return panelsState.PanelGroupList.First(x => x.Key == RightPanelGroupKey);
    }

    public static PanelGroup GetBottomPanelGroup(PanelsState panelsState)
    {
        return panelsState.PanelGroupList.First(x => x.Key == BottomPanelGroupKey);
    }
}
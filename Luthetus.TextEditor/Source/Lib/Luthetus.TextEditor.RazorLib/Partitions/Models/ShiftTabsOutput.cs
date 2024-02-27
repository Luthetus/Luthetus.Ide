namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

internal partial class Track
{
    private class ShiftTabsOutput
    {
        public ShiftTabsOutput(int relativeTabIndex, List<int> mutableTabList)
        {
            RelativeTabIndex = relativeTabIndex;
            MutableTabList = mutableTabList;
        }

        public int RelativeTabIndex { get; }
        public List<int> MutableTabList { get; }
    }
}

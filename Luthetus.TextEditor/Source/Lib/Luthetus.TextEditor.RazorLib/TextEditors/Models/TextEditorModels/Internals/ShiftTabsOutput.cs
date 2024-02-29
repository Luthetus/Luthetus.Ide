namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels.Internals;

public partial class TextEditorModelModifier
{
    internal class ShiftTabsOutput
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
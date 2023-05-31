namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.Razor.TextEditorCase;

public class TEST_delayed_text_insertion
{
    public TEST_delayed_text_insertion(
        string content,
        int offset)
    {
        Content = content;
        Offset = offset;
    }

    public string Content { get; set; }
    public int Offset { get; set; }
    public int StartInsertionPositionIndexInclusive { get; set; }
    public int EndInsertionPositionIndexExclusive { get; set; }
}

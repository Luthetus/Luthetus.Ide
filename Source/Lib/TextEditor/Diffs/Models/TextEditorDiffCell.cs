namespace Luthetus.TextEditor.RazorLib.Diffs.Models;

public class TextEditorDiffCell
{
    public TextEditorDiffCell(
        char? beforeCharValue,
        char? afterCharValue,
        int weight,
        bool isSourceOfRowWeight)
    {
        BeforeCharValue = beforeCharValue;
        AfterCharValue = afterCharValue;
        Weight = weight;
        IsSourceOfRowWeight = isSourceOfRowWeight;
    }

    public char? BeforeCharValue { get; }
    public char? AfterCharValue { get; }
    public int Weight { get; set; }
    public bool IsSourceOfRowWeight { get; }
}
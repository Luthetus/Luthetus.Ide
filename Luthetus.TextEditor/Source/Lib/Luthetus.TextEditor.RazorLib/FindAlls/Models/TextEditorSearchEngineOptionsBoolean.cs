namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public class TextEditorFindAllOptionsBoolean
{
    public TextEditorFindAllOptionsBoolean(string displayName)
    {
        DisplayName = displayName;
    }

    public string DisplayName { get; }
    public bool Value { get; set; }
}
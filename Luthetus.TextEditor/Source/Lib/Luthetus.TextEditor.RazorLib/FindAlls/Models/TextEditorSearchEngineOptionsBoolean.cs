namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public class TextEditorSearchEngineOptionsBoolean
{
    public TextEditorSearchEngineOptionsBoolean(string displayName)
    {
        DisplayName = displayName;
    }

    public string DisplayName { get; }
    public bool Value { get; set; }
}
namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public class TextEditorFindAllOptions
{
    public TextEditorFindAllOptions()
    {
        MatchCase = new("Match case");
        MatchWholeWord = new("Match whole word");
        UseRegularExpressions = new("Use regular expressions");
        IncludeExternalItems = new("Include external items");
        IncludeMiscellaneousFiles = new("Include miscellaneous files");
        AppendResults = new("Append Results");
    }

    public TextEditorFindAllOptionsBoolean MatchCase { get; }
    public TextEditorFindAllOptionsBoolean MatchWholeWord { get; }
    public TextEditorFindAllOptionsBoolean UseRegularExpressions { get; }
    public TextEditorFindAllOptionsBoolean IncludeExternalItems { get; }
    public TextEditorFindAllOptionsBoolean IncludeMiscellaneousFiles { get; }
    public TextEditorFindAllOptionsBoolean AppendResults { get; }
}
